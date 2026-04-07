// 購物車也只發一次請求
let _cartPromise = null;
function fetchCartOnce() {
    if (!_cartPromise) {
        _cartPromise = (async () => {
            const res = await fetch("/Cart/GetCartItems");
            const ct = res.headers.get('content-type') || '';
            if (!res.ok || !ct.includes('application/json')) return [];
            return await res.json();
        })();
    }
    return _cartPromise;
}

// 呼叫API的全域變數
const cartService = {
    // 1. 取得購物車訂單
    async getItem() {
        return await fetchCartOnce();
    },
    // 2. 數量加減 > 觸發全域事件，通知所有Vue購物車數量變動與更新
    async updateQty(productId, change=1) {
        const res = await fetch('/Cart/Add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId: productId, quantity: change })
        });
        if (res.ok) {
            _cartPromise = null;  // 清掉快取，下次會重新 fetch
            window.dispatchEvent(new Event('update-cart'));
            return true;
        }
        return false;
    },
    // 3. 移除商品 > 觸發全域事件，通知所有Vue購物車數量變動與更新
    async removeItem(productId) {
        const res = await fetch('/Cart/Remove', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productId)
        });
        if (res.ok) {
            _cartPromise = null;  // 清掉快取，下次會重新 fetch
            window.dispatchEvent(new Event('update-cart'));
            return true;
        }
        return false;
    }
}


// Vue 共用邏輯 放在Mix全域變數物件中
const CartMixin = {
    data() {
        return {
            cart: [],
            shippingFee: 0,
            couponCode: '',
            appliedDiscount: 0
        };
    },
    // 1. computed > totalPrice、totalCount、grandTotal
    computed: {
        //品項金額小計
        totalPrice() {
            return this.cart.reduce((sum, item) =>
            sum + (item.price * item.quantity), 0);
        },
        //買幾項
        totalCount() {
            return this.cart.reduce((sum, item) =>
                sum + item.quantity, 0);
        },
        //品項金額-折扣+運費
        grandTotal() {
            const fee = this.selectedShipping ? this.selectedShipping.fee : this.shippingFee;
            const total = this.totalPrice - this.appliedDiscount + fee;
            return total > 0 ? total : 0; // 確保不會出現負數
        }
    },
    // 2. method > 
    methods: {
        //refreshCart(叫用上面API全域變數的取得購物車訂單)
        async refreshCart() {
            this.cart = await cartService.getItem();
            //this.cart = data;
        },

        //加入購物車
        async addCart(product) {
            const selectedQty = product.qty || 1;
            const addSuccess = await cartService.updateQty(product.productId, selectedQty);

            if (addSuccess) {
                product.quantity = 1; //把畫面數字重設回1
                alert(`商品${product.productName}選購${selectedQty}件 已加入購物車!`);
            }
        },
        
        //全域通用的數量加減邏輯
        async updateQuantity(productId, change) {
            const item = this.cart.find(i => i.productId === productId);
            if (item.quantity === 1 && change === -1) {
                await this.deleteItem(productId);
            } else {
                await cartService.updateQty(productId, change);
            }
        },

        //刪除商品
        async deleteItem(productId) {
            if (confirm('確定移除商品?')) {
                await cartService.removeItem(productId);
            }
        },

        //套用優惠券
        async checkCoupon(isAutoRun = false) {
            if (!this.couponCode || this.couponCode.trim() === "") {
                return;
            }
            try {
                const res = await fetch('/api/CouponApi/CheckCoupon', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ couponCode: this.couponCode })
                });
                const result = await res.json();
                if (result.success) {
                    this.appliedDiscount = result.discount;
                    console.log("優惠券套用成功");
                } else {
                    this.appliedDiscount = 0;
                    alert(result.message || "優惠碼無效");
                }
            } catch (e) {
                if (!isAutoRun) {
                    alert("驗證優惠碼時發生錯誤");
                }
                return;
            }
        },

        formatNumber(num) {
            if (!num) return '0';
            return num.toLocaleString('zh-TW'); // 強制台灣格式
        }
    },
    //3. mounted 掛上addEventListener
    mounted() {
        this.refreshCart();
        window.addEventListener('update-cart', () => this.refreshCart());
    }   
}


// 呼叫API的全域變數
const cartService = {
    // 1. 取得購物車訂單
    async getItem() {
        const res = await fetch("/Cart/GetCartItems") //GET
        return await res.json();
    },
    // 2. 數量加減 > 觸發全域事件，通知所有Vue購物車數量變動與更新
    async updateQty(productId, change=1) {
        const res = await fetch('/Cart/Add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId: productId, quantity: change })
        });
        if (res.ok) {
            //await this.getItem(); 
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
            //await this.getItem();
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
            shippingFee:120
        };
    },
    // 1. computed > totalPrice、totalCount
    computed: {
        //totalPrice
        totalPrice() {
            return this.cart.reduce((sum, item) =>
            sum + (item.price * item.quantity), 0);
        },
        //totalCount
        totalCount() {
            return this.cart.reduce((sum, item) =>
                sum + item.quantity, 0);
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

        formatNumber(num) {
            if (!num) return '0';
            return num.toLocaleString('zh-TW'); // 強制台灣格式
        }
    },
    //3. mounted 掛上addEventListener
    mounted() {
        this.refreshCart();
        window.addEventListener('update-cart', () => this.refreshCart());
        //window.addEventListener(new Event('update-Cart'));
    }   
}


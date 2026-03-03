var products_swiper = new Swiper(".products-carousel2", {
    slidesPerView: 5,
    spaceBetween: 30,
    speed: 500,
    navigation: {
        nextEl: ".products-carousel2-next",
        prevEl: ".products-carousel2-prev",
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 3,
        },
        991: {
            slidesPerView: 4,
        },
        1200: {
            slidesPerView: 6,
        },
    }
});
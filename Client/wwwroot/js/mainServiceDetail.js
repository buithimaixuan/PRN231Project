console.log('body-content offset:', $('.body-content').offset().top);
console.log('scroll position:', $(window).scrollTop());
console.log('footer offset:', $('.footer--section').offset().top);

function showModalReview() {
    $('.modal-review').modal('show');
}

if ($('.body-content').offset().top <= 50 + $(window).scrollTop()) {
    console.log("kitra");
    $('.paid-course').addClass('show-fixed');
    $('.paid-course').removeClass('show-bottom');
}

if ($('.footer--section').offset().top <= $(window).scrollTop() + $(window).height()) {
    $('.paid-course').addClass('show-bottom');
    $('.paid-course').removeClass('show-fixed');
}

$(window).on('scroll', function () {
    if ($('.body-content').offset().top <= 50 + $(this).scrollTop()) {
        $('.paid-course').addClass('show-fixed');
        $('.paid-course').removeClass('show-bottom');
    } else {
        $('.paid-course').removeClass('show-fixed');
    }

    if ($('.footer--section').offset().top <= $(this).scrollTop() + $(window).height()) {
        $('.paid-course').addClass('show-bottom');
        $('.paid-course').removeClass('show-fixed');
    } else {
        $('.paid-course').removeClass('show-bottom');
    }
})

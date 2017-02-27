$(window).bind('scroll', function () {
    if ($(window).scrollTop() > 150) {
        $('#mainMenu').addClass('navbar-fixed-top');
    } else {
        $('#mainMenu').removeClass('navbar-fixed-top');
    }
});
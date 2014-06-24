$(function () {
    var nav = $('#nav-follow');
    if (nav.size() == 0) return;
    
    var stickyHeaderTop = nav.offset().top;
    $(window).scroll(function () {
        if ($(window).scrollTop() > stickyHeaderTop) {
            $('#nav-follow').addClass('nav-follow-fixed');
        } else {
            $('#nav-follow').removeClass('nav-follow-fixed');
        }
    });
});
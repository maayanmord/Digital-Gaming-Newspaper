"use strict";

// Toggle .header-scrolled class to #header when page is scrolled
$(window).scroll(() => {
    if ($(this).scrollTop() > 50) {
        $('#header').addClass('header-scrolled');
    } else {
        $('#header').removeClass('header-scrolled');
    }
});

if ($(window).scrollTop() > 50) {
    $('#header').addClass('header-scrolled');
}
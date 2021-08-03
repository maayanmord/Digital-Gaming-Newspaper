"use strict";

$(function () {
    var img = new Image();
    img.onload = function () {
        var ctx = $("#logoCanvas").get(0).getContext("2d");
        ctx.drawImage(img, 0, 0, 125, 45);
    }

    img.src = '/images/logo.png'

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
});
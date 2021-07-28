"use strict";

$(function () {
    /*var c = document.getElementById("logoCanvas");*/
    var ctx = $("#logoCanvas").get(0).getContext("2d");
    var img = $("#logo").get(0);
    ctx.drawImage(img, 50, 30, 170, 93);

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
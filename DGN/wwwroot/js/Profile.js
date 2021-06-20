
$(function () {

    function viewEdit() {
        var editProperties = $("#EditTemplate").html();
        $("#ViewProperties").html(editProperties);
        var editAbout = $("#EditAboutTemplate").html();
        $("#EditAbout").html(editAbout);
    }  

    $("#EditButton").click(function () {
        viewEdit();
    });

})






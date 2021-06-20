
$(function () {

    function viewEdit() {
        var item = $("#EditTemplate").html();
        $("#ViewProperties").html(item);
    }    

    $("#EditButton").click(function () {
        viewEdit();
    });

})






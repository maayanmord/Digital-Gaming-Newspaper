$(function () {

})

function isValidForm() {
    var currPass = $('#currPassword').val();
    var newPass = $('#newPassword').val();
    var confirmNewPass = $("#confirmNewPassword").val();

    $("#currPasswordError").html("");
    $("#message").html("");

    if (currPass == '' || newPass == '' || confirmNewPass == '') {
        $('#message').html("Please fill all the required fields")
        return false;
    }
    if (newPass != confirmNewPass) {
        $('#message').html("Passwords don't match")
        return false;
    }

    return true;
}
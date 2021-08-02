$(function () {

})

function isValidForm() {
    const regex = new RegExp("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])\\S{8,}$");
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
    if (!regex.test(newPass)) {
        $('#message').html("The minumum requierments for password are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character")
        return false;
    }

    return true;
}
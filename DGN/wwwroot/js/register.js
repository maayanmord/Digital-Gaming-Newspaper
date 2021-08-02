$(function () {
    $("#GenerateUsernameButton").click(function () {
        $.ajax({
            method: "GET",
            url: "https://randomuser.me/api/",
            success: function (data) {
                $("#generateUsernameError").html('');
                $("#Username").val(data.results[0].login.username);
            },
            error: function (data) {
                $("#generateUsernameError").html('Error occured while retrieving generated username').css('color', 'red');
            }
        });
    });
});

function isValidForm() {
    var isValid = true;
    var pass = $('#Password').val();
    var confirmPass = $('#confirmPassword').val();
    $('#passMessage').html("");
    $('#confPassMessage').html("");

    if (pass == '') {
        $('#passMessage').html("Please enter your password")
        isValid = false;
    }
    if (confirmPass == '') {
        $('#confPassMessage').html("Please enter your password again")
        isValid = false;
    }
    if (pass != '' && confirmPass != '' && pass != confirmPass) {
        $('#confPassMessage').html("Passwords don't match");
        isValid = false;
    }

    return isValid;
}
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
    const regex = new RegExp("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])\\S{8,}$");
    var isValid = true;
    var pass = $('#Password').val();
    var confirmPass = $('#confirmPassword').val();
    $('#passMessage').html("");
    $('#confPassMessage').html("");
    $('#errorMessage').html("");

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

    // Don't check regex validate if something went wrong before this validation
    if (!isValid) {
        return isValid
    }

    if (!regex.test(pass)) {
        $('#confPassMessage').html("The minumum requierments for password are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
        isValid = false;
    }

    return isValid;
}
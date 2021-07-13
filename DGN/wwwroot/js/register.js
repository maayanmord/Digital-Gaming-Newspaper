$(function () {
    var passwordsMatch = false;
    var passwordNOTEmpty = false;
    var confPasswordNOTEmpty = false;

    $('#Password, #confirmPassword').on('keyup', function () {
        if ($('#Password').val() == $('#confirmPassword').val()) {
            $('#notMatchPassMessage').html('');
            passwordsMatch = true;
        } else {
            $('#notMatchPassMessage').html('Passwords Not Matching').css('color', 'red');
            passwordsMatch = false;
        }

        if ($('#Password').val() != '') {
            $('#passMessage').html('');
            passwordNOTEmpty = true;
        } else {
            $('#passMessage').html('You forget to enter the password!').css('color', 'red');
            passwordNOTEmpty = false;
        }

        if ($('#confirmPassword').val() != '') {
            $('#confPassMessage').html('');
            confPasswordNOTEmpty = true;
        } else {
            $('#confPassMessage').html('Please enter the password again!').css('color', 'red');
            confPasswordNOTEmpty = false;
        }

        // The submit button is enable only when the passwords match AND both of the passwords fields are not empty.
        $('#submit').prop('disabled', (!(passwordsMatch && passwordNOTEmpty && confPasswordNOTEmpty)));
    });
})
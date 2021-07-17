$(function () {
    var passwordMatch = false;
    var passwordNOTEmpty = false;

    $('#newPassword, #confirmNewPassword, #currPassword').on('keyup', function () {
        if ($('#newPassword').val() == $('#confirmNewPassword').val()) {
            passwordMatch = true;
        } else {
            passwordMatch = false;
        }

        if ($('#newPassword').val() == '' || $('#confirmNewPassword').val() == '' || $('#currPassword').val() == '' ) {
            passwordNOTEmpty = false;
        } else {
            passwordNOTEmpty = true;
        }

        if (passwordMatch && passwordNOTEmpty) {
            $('#submit').prop('disabled', false)
            $('#message').html('');
        } else {
            $('#submit').prop('disabled', true)
            if (!passwordMatch) {
                $('#message').html('Passwords Not Matching').css('color', 'red');
            } else {
                $('#message').html('Some fields are missing!').css('color', 'red');
            }
            
        }
    });
})
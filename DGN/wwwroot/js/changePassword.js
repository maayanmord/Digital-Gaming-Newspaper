$(function () {
    $('#newPassword, #confirmNewPassword').on('keyup', function () {
        if ($('#newPassword').val() == $('#confirmNewPassword').val()) {
            $('#message').html('');
            $('#submit').prop('disabled', false)
        } else {
            $('#message').html('Passwords Not Matching').css('color', 'red');
            $('#submit').prop('disabled', true)
        }
    });
})
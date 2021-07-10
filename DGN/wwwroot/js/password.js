$('#newPassword, #confirmNewPassword').on('keyup', function () {
    if ($('#newPassword').val() == $('#confirmNewPassword').val()) {
        $('#message').html('');
    } else
        $('#message').html('Passwords Not Matching').css('color', 'red');
});
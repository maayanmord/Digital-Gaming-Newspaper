$(function () {
    $("#searchForm").submit(function (e) {
        e.preventDefault();
        var categoryName = $('#categoryName').val();

        $.ajax({
            type: "POST",
            url: "/Categories/Search",
            data: {
                categoryName,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                $('tbody').html('');
                $("#searchFormError").html('');

                var template = $('#categoryTemplate').html();

                $.each(data, function (i, val) {

                    var temp = template;

                    $.each(val, function (key, value) {
                        temp = temp.replaceAll('{' + key + '}', value);
                    });

                    $('tbody').append(temp);
                });
            },
            error: function (data) {
                $("#searchFormError").html('Error occured while searching').css('color', 'red');
            }
        });
    });
});
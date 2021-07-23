$(function () {
    $('form').submit(function (e) {
        e.preventDefault();

        var query = $('#query').val();

        $.ajax({
            // method : 'post',
            url: '/Branches/Search',
            data: { 'query': query }
        }).done(function (data) {
            $('tbody').html('');

            var template = $('#hidden-template').html();

            $.each(data, function (i, val) {

                var temp = template;

                $.each(val, function (key, value) {
                    temp = temp.replaceAll('{' + key + '}', value);
                });

                $('tbody').append(temp);
            });
        });
    });
});
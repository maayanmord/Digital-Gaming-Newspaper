$(function () {
    $('#searchForm').submit(function (e) {
        e.preventDefault();

        var query = $('#query').val();

        $.ajax({
            // method : 'post',
            url: '/Users/Search',
            data: { 'query': query }
        }).done(function (data) {
            $('tbody').html('');

            var template = $('#hidden-template').html();

            $.each(data, function (i, val) {

                var temp = template;

                $.each(val, function (key, value) {
                    temp = temp.replaceAll('{' + key + '}', value);
                });

                if (val.canDelete) {
                    temp = temp.replaceAll('{delete}', " <span>|</span> <a href ='Users/Delete/" + val.id + "'>Delete</a>");
                } else {
                    temp = temp.replaceAll('{delete}', "");
                }

                $('tbody').append(temp);
            });
        });
    });
});
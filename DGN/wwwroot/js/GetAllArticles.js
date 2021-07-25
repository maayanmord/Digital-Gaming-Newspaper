
$(async function () {

    // Get all the categories from the db
    const getAllCategories = () => {
        let categoriesArray = [];
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "GET",
                url: "/Categories/GetAll",
                success: function (data) {
                    data.map((m => {
                        categoriesArray.push({
                            categoryName: m.categoryName,
                            id: m.id
                        })
                    }));
                    resolve(categoriesArray)
                },
                error: function (err) {
                    resolve(null);
                }
            });
        })
    }

    // Fetch all categories from server..
    let allCategoriesArray = await getAllCategories();

    $('#categorySelect').append($('<option value="-1" selected)>All Categories</option>'));

    // Set the categories inside dropdown..
    $.each(allCategoriesArray, function (i, p) {
        $('#categorySelect').append($('<option value="' + p.id + '"></option>').html(p.categoryName));
    });

    // Get the categories from db using the advanced search
    const advancedSearch = () => {

        let category = $('#categorySelect').val();
        let author = $('#authorInput').val();
        let containsWordsInput = $('#containsWordsInput').val();

        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/Articles/AdvancedSearch",
                data: {
                    categoryId: category === undefined ? null : parseInt(category),
                    author: (author === undefined || author.length === 0) ? null : author,
                    title: (containsWordsInput === undefined || containsWordsInput.length === 0) ? null : containsWordsInput,
                },
                success: function (data) {
                    resolve(data)
                },
                error: function (err) {
                    resolve(null);
                }
            });
        })
    }

    // Get the categories from db using the advanced search
    const normalSearch = () => {

        let containsWordsInTitle = $('#normalSearchInput').val();

        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/Articles/AdvancedSearch",
                data: {
                    categoryId: null,
                    author: null,
                    title: (containsWordsInTitle === undefined || containsWordsInTitle.length === 0) ? null : containsWordsInTitle,
                },
                success: function (data) {
                    resolve(data)
                },
                error: function (err) {
                    resolve(null);
                }
            });
        })
    }

    // Refresh Template according to fetched data..
    const refreshTemplate = (data) => {

        var template = $('#hidden-template').html();
        $('#all-articles').html('');

        $.each(data, function (i, val) {
            var temp = template;
            $.each(val, function (key, value) {
                temp = temp.replaceAll('{' + key + '}', value);
            });

            $('#all-articles').append(temp);
        });
    }


    $('#advancedSubmitButton').click(async (e) => {
        e.preventDefault();
        let advancedSearchResponse = await advancedSearch();
        refreshTemplate(advancedSearchResponse);
    });

    $('#normalSubmitButton').click(async (e) => {
        let normalSearchResponse = await normalSearch();
        refreshTemplate(normalSearchResponse);
    });


})
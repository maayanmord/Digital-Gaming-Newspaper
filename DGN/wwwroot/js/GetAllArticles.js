
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

    // Set the categories inside dropdown..
    $.each(allCategoriesArray, function (i, p) {
        $('#categorySelect').append($('<option value="' + p.id + '"' + (i === 0 ? ' selected' : '') + '></option>').html(p.categoryName));
    });

    // Get the categories from db using the advanced search
    const advancedSearch = () => {

        let category = $('#categorySelect option[selected]').val();
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


    $('#advancedSubmitButton').click(async (e) => {
        e.preventDefault();
        let advancedSearchResponse = await advancedSearch();
        console.log(advancedSearchResponse);
    });


})
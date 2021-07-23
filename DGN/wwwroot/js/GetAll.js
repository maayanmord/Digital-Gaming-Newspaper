
$(async function () {

    // Get all the categories from the db
    const getAllCategories = () => {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "GET",
                url: "/Categories/GetAll",
                success: function (data) {
                    resolve(data.map((m => { return m.categoryName; })))
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
        $('#categorySelect').append($('<option' + (i === 0 ? ' selected' : '') + '></option>').val(p).html(p));
    });

    const advancedSearch = () => {

        let category = $('#categorySelect option[selected]').val();
        let author = $('#authorInput').val();
        let containsWordsInput = $('#containsWordsInput').val();

        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "/Categories/GetAll",
                data: {
                    Body: commentBody,
                    RelatedArticleId: ArticleId,
                    __RequestVerificationToken: gettoken()
                },
                success: function (data) {
                    resolve(data.map((m => { return m.categoryName; })))
                },
                error: function (err) {
                    resolve(null);
                }
            });
        })
    }
  

})
$(function () {
    //
    // graph for articles per categories
    //

    function random_colors(num_of_colors)
    {
        var colors = []

        var index = 0
        var letters = '0123456789ABCDEF';
        var color
        while (index != num_of_colors) {
            color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            if (!colors.includes(color)) {
                index++;
                colors.push(color);
            }
        }
        return colors;
    }

    var max_article_count = 0;
    data = []
    $.ajax({
        type: "GET",
        url: "/Articles/GetArticlesByCategory",
        success: function (dataJson) {
            colors = random_colors(dataJson.length);
            i = 0;
            dataJson.forEach(currCategory => {
                if (currCategory.articlesInCategory > max_article_count) {
                    max_article_count = currCategory.articlesInCategory;
                }
                data.push({
                    "categoryName": currCategory.categoryName,
                    "articlesInCategory": currCategory.articlesInCategory,
                    "color": colors[i]
                });
                i++;
            })
        },
        error: function () {
            $("#alert-body").html('ERROR');
        }
    }).done(function () {
        articles_per_category(data, max_article_count)
    });

    function articles_per_category(data, max_article_count) {
        // set the dimensions and margins of the graph
        var margin = { top: 10, right: 50, bottom: 90, left: 40 },
            width = 460 - margin.left - margin.right,
            height = 450 - margin.top - margin.bottom;

        // append the svg object to the body of the page
        var svg = d3.select("#articles_per_category")
            .append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")");
        // X axis
        var x = d3.scaleBand()
            .range([0, width])
            .domain(data.map(function (d) { return d.categoryName; }))
            .padding(0.2);
        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x))
            .selectAll("text")
            .attr("transform", "translate(-10,0)rotate(-45)")
            .style("text-anchor", "end");

        // Add Y axis
        var y = d3.scaleLinear()
            .domain([0, Math.max(max_article_count, 5)])
            .range([height, 0]);
        svg.append("g")
            .call(d3.axisLeft(y));

        // Bars
        svg.selectAll("mybar")
            .data(data)
            .enter()
            .append("rect")
            .attr("x", function (d) { return x(d.categoryName); })
            .attr("width", x.bandwidth())
            .attr('fill', function (d) { return d.color; })
            // .attr("fill", "#69b3a2")
            // no bar at the beginning thus:
            .attr("height", function (d) { return height - y(0); }) // always equal to 0
            .attr("y", function (d) { return y(0); })

        // Animation
        svg.selectAll("rect")
            .transition()
            .duration(800)
            .attr("y", function (d) { return y(d.articlesInCategory); })
            .attr("height", function (d) { return height - y(d.articlesInCategory); })
            .delay(function (d, i) { console.log(i); return (i * 100) });
    }

    //
    // Graph for articles over time 
    //

    var ArticlesByDate = []
    var max_articles = 0;
    $.ajax({
        type: "GET",
        url: "/Articles/GetArticlesDates",
        success: function (dataJson) {
            dataJson.forEach(currDate => {
                // if more than one article were created the same time
                // removing old value to add new amount to this time.
                var index = ArticlesByDate.indexOf({ "date": Date.parse(currDate), "amount": max_articles })
                if (index > -1) {
                    ArticlesByDate.splice(index, 1);
                }

                max_articles++;
                ArticlesByDate.push({
                    "date": Date.parse(currDate),
                    "amount": max_articles
                });
            });
        },
        error: function () {
            $("#alert-body").html('ERROR');
        }
    }).done(function () {
        articles_over_time(ArticlesByDate, max_articles);
    });


    function articles_over_time(data, max_articles) {

        // set the dimensions and margins of the graph
        var margin = { top: 10, right: 50, bottom: 90, left: 40 },
            width = 460 - margin.left - margin.right,
            height = 450 - margin.top - margin.bottom;

        // append the svg object to the body of the page
        var svg = d3.select("#articles_over_time")
            .append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scaleTime()
            .domain(d3.extent(data, function (d) { return d.date; }))
            .range([0, width]);
        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x));
                // Add Y axis
                var y = d3.scaleLinear()
                    .domain([0, Math.max(max_articles, 5)])
                    .range([height, 0]);
                svg.append("g")
                    .call(d3.axisLeft(y));
                // Add the line
                svg.append("path")
                    .datum(data)
                    .attr("fill", "none")
                    .attr("stroke", "#69b3a2")
                    .attr("stroke-width", 1.5)
                    .attr("d", d3.line()
                        .x(function (d) { return x(d.date) })
                        .y(function (d) { return y(d.amount) })
                    )
                // Add the points
                svg.append("g")
                    .selectAll("dot")
                    .data(data)
                    .enter()
                    .append("circle")
                    .attr("cx", function (d) { return x(d.date) })
                    .attr("cy", function (d) { return y(d.amount) })
                    .attr("r", 5)
                    .attr("fill", "#69b3a2")
    }
})
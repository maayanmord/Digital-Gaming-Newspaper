// First statistic: articles_per_category
// set the dimensions and margins of the graph
var width = 450
height = 450
margin = 40

// The radius of the pieplot is half the width or half the height (smallest one), subtract a bit of margin.
var radius = Math.min(width, height) / 2 - margin

// append the svg_articles_per_category object to the div called 'articles_per_category'
var svg_articles_per_category = d3.select("#articles_per_category")
    .append("svg")
    .attr("width", width)
    .attr("height", height)
    .append("g")
    .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

// Get the data
var ArticlesByCategorty = {}
var ArticlesCount = 0;

$.ajax({
    type: "GET",
    url: "/Articles/GetArticlesByCategory",
    success: function (dataJson) {
        dataJson.forEach(currCategory => {
            ArticlesCount += currCategory.articlesInCategory;
            ArticlesByCategorty[currCategory.categoryName] = currCategory.articlesInCategory;
        })
    },
    error: function () {
        $("#alert-body").html('ERROR');
    }
}).done(function() { 
    DrawPiechart(ArticlesByCategorty)
});

function DrawPiechart(data) {
    // set the color scale
    var color = d3.scaleOrdinal()
        .domain(data)
        .range(d3.schemeSet2);

    // Compute the position of each group on the pie:
    var pie = d3.pie()
        .value(function (d) { return d.value; })
    var data_ready = pie(d3.entries(data))

    // shape helper to build arcs:
    var arcGenerator = d3.arc()
        .innerRadius(0)
        .outerRadius(radius)

    // Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
    svg_articles_per_category
        .selectAll('mySlices')
        .data(data_ready)
        .enter()
        .append('path')
        .attr('d', arcGenerator)
        .attr('fill', function (d) { return (color(d.data.key)) })
        .attr("stroke", "black")
        .style("stroke-width", "2px")
        .style("opacity", 0.7)

    // Add the annotation. Use the centroid method to get the best coordinates
    svg_articles_per_category
        .selectAll('mySlices')
        .data(data_ready)
        .enter()
        .append('text')
        .text(function (d) { return "\"" + d.data.key + "\": " + ((d.data.value * 100) / ArticlesCount).toFixed(2) + "%" })
        .attr("transform", function (d) { return "translate(" + arcGenerator.centroid(d) + ")"; })
        .style("text-anchor", "middle")
        .style("font-size", 17)
};


// /////////////////////////////////////////////////////////
// Second statistic: articles_over_time

var ArticlesByDate = []

$.ajax({
    type: "GET",
    url: "/Articles/GetArticlesDates",
    success: function (dataJson) {
        dataJson.forEach(currDate => {
            ArticlesByDate.push(Date.parse(currDate));
        });        
    },
    error: function () {
        $("#alert-body").html('ERROR');
    }
}).done(function () {
    DrawTimeline(ArticlesByDate);
});

function DrawTimeline(data) {
    var readableDates = [];

    data.forEach(curr => {
        date = new Date(curr)
        date = date.toLocaleString('en-US')
        readableDates.push(date);
    })

    var min = d3.min(data);
    var max = d3.max(data);
    var domain = [min, max];

    // set the dimensions and margins of the graph
    var margin = { top: 10, right: 30, bottom: 30, left: 40 },
        width = 450 - margin.left - margin.right,
        height = 450 - margin.top - margin.bottom;

    // The number of bins
    Nbin = 10;

    var x = d3
        .scaleTime()
        .domain(domain)
        .range([0, width]);

    // Build the histogram function and gets the bins
    var histogram = d3
        .histogram()
        .domain(x.domain()) // then the domain of the graphic
        .thresholds(x.ticks(Nbin)); // then the numbers of bins

    // And apply this function to data to get the bins
    var bins = histogram(data);

    // Build the top element of the graphic
    // Add the svg_articles_over_time element to the body and set the dimensions and margins of the graph
    var svg_articles_over_time = d3
        .select("#articles_over_time")
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    svg_articles_over_time
        .append("g")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(x));

    var y = d3
        .scaleLinear()
        .range([height, 0])
        .domain([
            0,
            d3.max(bins, function (d) {
                return d.length;
            })
        ]);

    svg_articles_over_time.append("g").call(d3.axisLeft(y));

    svg_articles_over_time
        .selectAll("rect")
        .data(bins)
        .enter()
        .append("rect")
        .attr("x", 1)
        .attr("transform", function (d) {
            return "translate(" + x(d.x0) + "," + y(d.length) + ")";
        })
        .attr("width", function (d) {
            return x(d.x1) - x(d.x0) - 1;
        })
        .attr("height", function (d) {
            return height - y(d.length);
        })
        .style("fill", "#69b3a2");
};
function calendar(myData, control, months_names, days_names, ranges, units, split) {
    
    var calendarRows = function (month) {
        var m = d3.timeMonth.floor(month);
        return d3.timeWeeks(d3.timeWeek.floor(m), d3.timeMonth.offset(m, 1)).length;
    }

    var minDate = d3.min(myData, function (d) { return new Date(Date.UTC(parseInt(d.end.split("T")[0].split("-")[0]), parseInt(d.end.split("T")[0].split("-")[1])-1, parseInt(d.end.split("T")[0].split("-")[2])+1)); });
    var maxDate = d3.max(myData, function (d) { return new Date(Date.UTC(parseInt(d.end.split("T")[0].split("-")[0]), parseInt(d.end.split("T")[0].split("-")[1])-1, parseInt(d.end.split("T")[0].split("-")[2])+1)); });
   
    var width_full = (split ? $(document).width() / 2 : $(document).width()) * 0.9;

    var cellMargin = 2,
        cellSize = (split ? width_full / 17 : width_full / 59);

    var day = d3.time.format("%w"),
        week = d3.time.format("%U"),
        format = d3.time.format("%Y-%m-%d"),
        months = d3.timeMonth.range(d3.timeMonth.floor(minDate), maxDate);

    for (var i = 0; i < myData.length; i++) {
        myData[i].today = myData[i].end.slice(0, 10);
    }

    $("#" + control + ' svg').remove();
    
    var svg = d3.select("#" + control).selectAll("svg")
        .data(months)
        .enter().append("svg")
        .attr("class", "month")
        .attr("width", (cellSize * 7) + (cellMargin * 8))
        .attr("height", function (d) {
            var rows = calendarRows(d);
            return (cellSize * rows) + (cellMargin * (rows + 1)) + 45; // the 20 is for the month labels
        })
        .append("g");

    svg.append("text")
        .attr("class", "month-name")
        .attr("x", ((cellSize * 7) + (cellMargin * 8)) / 2)
        .attr("y", 15)
        .attr("text-anchor", "middle")
        .text(function (d) { return months_names[d.getMonth()]; });

    svg.selectAll("rect.day.names")
        .data(function (d, i) {            
            return d3.timeDays(new Date(2019,9,1), new Date(2019, 9, 8));
        })
        .enter().append("text")
        .attr("class", "day.name")
        .attr("x", function (d) {
            return (day(d) * cellSize) + (day(d) * cellMargin) + cellMargin;
        })
        .attr("y", 35)
        .text(function (d, i) { return days_names[i]; })

        ;

    var rect = svg.selectAll("rect.day")
        .data(function (d, i) {
            return d3.timeDays(d, new Date(d.getFullYear(), d.getMonth() + 1, 1));
        })
        .enter().append("rect")
        .attr("class", "day")
        .attr("width", cellSize)
        .attr("height", cellSize)
        .attr("rx", 3).attr("ry", 3) // rounded corners
        .attr("fill", '#eaeaea') // default light grey fill
        .attr("x", function (d) {
            return (day(d) * cellSize) + (day(d) * cellMargin) + cellMargin;
        })
        .attr("y", function (d) {
            return ((week(d) - week(new Date(d.getFullYear(), d.getMonth(), 1))) * cellSize) +
                ((week(d) - week(new Date(d.getFullYear(), d.getMonth(), 1))) * cellMargin) +
                cellMargin + 45;
        })
        .on("mouseover", function (d) {
            d3.select(this).classed('hover', true);
        })
        .on("mouseout", function (d) {
            d3.select(this).classed('hover', false);
        })
        .datum(format);

    var lookup = d3.nest()
        .key(function (d) { return d.today; })
        .rollup(function (leaves) { return leaves[0].data; })
        .entries(myData)
        ;

    var tooltip = d3.select("body").append("div")
        .attr("class", "tooltip")
        .style("opacity", 0);

    rect.style("fill", function (d) {
        var answer = '';
        var i = lookup.filter(function (item) { return d == item.key });
        if (i.length > 0) {
            var value = i[0].values.filter(function (item2) { return item2.measure.startsWith('yield'); })[0].avg;
            var r = ranges.filter(function (item2) { return item2.lower <= value && item2.upper >= value; })[0];
            answer = r == undefined ? '' : r.color;
        }
        return answer;
    })
        .on("mouseover", function (d, i) {
            var answer = '';
            var i = lookup.filter(function (item) { return d == item.key });
            if (i.length > 0) {
                var value = i[0].values.filter(function (item2) { return item2.measure.startsWith('yield'); })[0].avg;
                answer = value;
                tooltip.transition()
                    .duration(200)
                    .style("opacity", .9);
                tooltip.html(d + ': ' + d3.format('0f')(answer) + ' ' + units)
                    .style("left", d3.event.pageX + 20 + "px")
                    .style("top", d3.event.pageY + "px");
            }
        })
        .on("mouseout", function (d) {
            tooltip.transition()
                .duration(500)
                .style("opacity", 0);
        })
        ;

}
/**
 * This class draws a calendar graphic
 * (Base) base: Configuration to render the graphic
 */
function Calendar(base, months_names, days_names) {
    var base = base;
    var counter = 0;
    var current_month = new Date().getMonth();
    var months_names = months_names;
    var days_names = days_names;
    var cell_width = 0;

    var cellColorForCurrentMonth = '#EAEAEA';
    var cellColorForPreviousMonth = '#FFFFFF';

    function incrementCounter() {counter += 1; }
    function decrementCounter() { counter -= 1; }
    function monthToDisplay() {
        var dateToDisplay = new Date();
        // We use the counter that keep tracks of "back" and "forward" presses to get the month to display.
        dateToDisplay.setMonth(current_month + counter);
        return dateToDisplay.getMonth();
    }
    function monthToDisplayAsText() { return monthNames[monthToDisplay()]; }
    function yearToDisplay() {
        var dateToDisplay = new Date();
        // We use the counter that keep tracks of "back" and "forward" presses to get the year to display.
        dateToDisplay.setMonth(current_month + counter);
        return dateToDisplay.getFullYear();
    }
    
    // This function generates all the days of the month. But since we have a 7 by 5 grid, we also need to get some of
    // the days from the previous month and the next month. This way our grid will have all its cells filled. The days
    // from the previous or the next month will have a different color though. 
    function daysInMonth() {
        var daysArray = [];

        var firstDayOfTheWeek = new Date(yearToDisplay(), monthToDisplay(), 1).getDay();
        var daysInPreviousMonth = new Date(yearToDisplay(), monthToDisplay(), 0).getDate();
        // Lets say the first week of the current month is a Wednesday. Then we need to get 3 days from 
        // the end of the previous month. But we can't naively go from 29 - 31. We have to do it properly
        // depending on whether the last month was one that had 31 days, 30 days or 28.
        for (var i = 1; i <= firstDayOfTheWeek; i++) 
            daysArray.push([daysInPreviousMonth - firstDayOfTheWeek + i, cellColorForCurrentMonth]);
        

        // These are all the days in the current month.
        var daysInMonth = new Date(yearToDisplay(), monthToDisplay() + 1, 0).getDate();
        for (i = 1; i <= daysInMonth; i++) 
            daysArray.push([i, cellColorForPreviousMonth]);

        // Depending on how many days we have so far (from previous month and current), we will need
        // to get some days from next month. We can do this naively though, since all months start on
        // the 1st.
        var daysRequiredFromNextMonth = 35 - daysArray.length;

        for (i = 1; i <= daysRequiredFromNextMonth; i++) 
            daysArray.push([i, cellColorForCurrentMonth]);

        return daysArray.slice(0, 35);
    }

    function dateToYMD (date) {
        var d = date.getDate();
        var m = date.getMonth() + 1;
        var y = date.getFullYear();
        return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
    }
    function searchInData(date) {
        return base.data.filter(function (item) {
            return item.Fecha == date;
        });
    }

    function color() {
        return d3.scale.quantize()
            .domain([9000, 11000])
            //.domain([0, 12000])
            //.range(["hsl(0,100%,36%)", "hsl(130,100%,36%)"]);
            .range(["#A50026", "#F46D43", "#FEE08B", "#D9EF8B", "#66BD63", "#006837"]);
    }
};

/*
 * Get the width of a cell in the graphic
*/
Calendar.prototype.cell_width = function () { return this.base.width / 7; }
/*
 * Get the height of a cell in the graphic
*/
Calendar.prototype.cell_height = function () { return this.base.height / 7; }
/*
 * Method to create a array with data for the calendar. It builds the structure of a calendar (matrix 5x7)
*/
Calendar.prototype.generate_grid = function () {
    // We store the top left positions of a 7 by 5 grid. These positions will be our reference points for drawing
    // various objects such as the rectangular grids, the text indicating the date etc.
    var cellPositions = [];
    for (var y = 0; y < 5; y++) 
        for (var x = 0; x < 7; x++) 
            cellPositions.push([x * this.cell_width(), y * this.cell_height()]);
    return cellPositions;
}
/*
 * Method
*/
Calendar.prototype.get_data_month = function () {
    var randomData = [];
    var firstDayOfTheWeek = new Date(yearToDisplay(), monthToDisplay(), 1).getDay();
    var daysInPreviousMonth = new Date(yearToDisplay(), monthToDisplay(), 0).getDate();
    // Lets say the first week of the current month is a Wednesday. Then we need to get 3 days from 
    // the end of the previous month. But we can't naively go from 29 - 31. We have to do it properly
    // depending on whether the last month was one that had 31 days, 30 days or 28.
    for (var i = 1; i <= firstDayOfTheWeek; i++) {
        var date = new Date(yearToDisplay(), monthToDisplay() - 1, daysInPreviousMonth - firstDayOfTheWeek + i);
        var value = searchInData(dateToYMD(date));
        randomData.push(value);
    }

    // These are all the days in the current month.

    var daysInMonth = new Date(yearToDisplay(), monthToDisplay(), 0).getDate();
    for (i = 1; i <= daysInMonth; i++) {
        var date = new Date(yearToDisplay(), monthToDisplay(), i);
        var value = searchInData(dateToYMD(date));
        randomData.push(value);
    }

    // Depending on how many days we have so far (from previous month and current), we will need
    // to get some days from next month. We can do this naively though, since all months start on
    // the 1st.
    var daysRequiredFromNextMonth = 35 - randomData.length;
    for (i = 1; i <= daysRequiredFromNextMonth; i++) {
        var date = new Date(yearToDisplay(), monthToDisplay() + 1, i);
        var value = searchInData(dateToYMD(date));
        randomData.push(value);
    }
    return randomData;
}
/*
 * Dispatch the event to show next month
*/
Calendar.prototype.next_month = function () {

}

/*
 * Dispatch the event to show prev month
*/
Calendar.prototype.prev_month = function () {

}

/*
*/
Calendar.prototype.render_month = function () {
    // RENDERDAYSOFMONTH
    //$('#currentMonth').text(D3Graphics.CalendarGoogle.tools.monthToDisplayAsText() + ' ' + D3Graphics.CalendarGoogle.tools.yearToDisplay());
    // Get data
    var data = this.get_data_month();

    // We get the days for the month we need to display based on the number of times the user has pressed
    // the forward or backward button.
    var daysInMonthToDisplay = D3Graphics.CalendarGoogle.tools.daysInMonth();
    var cellPositions = D3Graphics.CalendarGoogle.tools.gridCellPositions();

    // All text elements representing the dates in the month are grouped together in the "datesGroup" element by the initalizing
    // function below. The initializing function is also responsible for drawing the rectangles that make up the grid.
    D3Graphics.CalendarGoogle.vars.datesGroup
        .selectAll("text")
        .data(daysInMonthToDisplay)
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + D3Graphics.CalendarGoogle.vars.gridXTranslation + "," + D3Graphics.CalendarGoogle.vars.gridYTranslation + ")")
        .text(function (d) { return d[0]; }); // Render text for the day of the week

    D3Graphics.CalendarGoogle.vars.chartsGroup
        .selectAll(".gcContent")
        .remove();

    D3Graphics.CalendarGoogle.vars.chartsGroup
        .selectAll("g.text")
        .data(daysInMonthToDisplay)
        .enter()
        .append("text")
        .attr("class", "gcContent")
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + (D3Graphics.CalendarGoogle.vars.gridXTranslation - 18) + "," + (D3Graphics.CalendarGoogle.vars.gridYTranslation + 40) + ")")
        .text(function (d, i) {
            return data[i].length > 0 ? 'Rendimiento esperado: ' : '';
        }); // Render text for the day of the week

    D3Graphics.CalendarGoogle.vars.chartsGroup
        .selectAll("g.text")
        .data(daysInMonthToDisplay)
        .enter()
        .append("text")
        .attr("class", "gcContent")
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + (D3Graphics.CalendarGoogle.vars.gridXTranslation - 18) + "," + (D3Graphics.CalendarGoogle.vars.gridYTranslation + 55) + ")")
        .text(function (d, i) {
            return data[i].length > 0 ? round(data[i][0].RendimientoPromedio) : '';
        })
        .style("font-size", '15px')
        .style("font-weight", 'bold'); // Render text for the day of the week

    D3Graphics.CalendarGoogle.vars.chartsGroup
        .selectAll("g.text")
        .data(daysInMonthToDisplay)
        .enter()
        .append("text")
        .attr("class", "gcContent")
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + (D3Graphics.CalendarGoogle.vars.gridXTranslation - 18) + "," + (D3Graphics.CalendarGoogle.vars.gridYTranslation + 70) + ")")
        .text(function (d, i) {
            return data[i].length > 0 ? 'Intervalo Rendimiento: ' : '';
        }); // Render text for the day of the week

    D3Graphics.CalendarGoogle.vars.chartsGroup
        .selectAll("g.text")
        .data(daysInMonthToDisplay)
        .enter()
        .append("text")
        .attr("class", "gcContent")
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + (D3Graphics.CalendarGoogle.vars.gridXTranslation - 18) + "," + (D3Graphics.CalendarGoogle.vars.gridYTranslation + 85) + ")")
        .text(function (d, i) {
            if (data[i].length > 0) {
                var low = parseFloat(data[i][0].RendimientoPromedio) - parseFloat(data[i][0].RendimientoDesviacion);
                var upper = parseFloat(data[i][0].RendimientoPromedio) + parseFloat(data[i][0].RendimientoDesviacion);
                return '[' + round(low) + ' - ' + round(upper) + ']'
            }
            else
                return '';
        })
        .style("font-size", '14px')
        .style("font-weight", 'bold'); // Render text for the day of the week

    var color = D3Graphics.CalendarGoogle.tools.color();

    D3Graphics.CalendarGoogle.vars.calendar
        .selectAll("rect")
        .data(daysInMonthToDisplay)
        // Here we change the color depending on whether the day is in the current month, the previous month or the next month.
        // The function that generates the dates for any given month will also specify the colors for days that are not part of the
        // current month. We just have to use it to fill the rectangle
        .style("fill", function (d, i) {
            var bg = '';
            if (d[1].indexOf('FFFFFF')) {
                bg += data[i].length > 0 ? color(data[i][0].RendimientoPromedio) : d[1];
            }
            else
                bg += d[1];
            return bg;
        });
},

/*
 * Method that render the graphic in a container
*/
Calendar.prototype.render = function () {
    var that = this;
    this.base.init(true, 0.5);

    /*
    if (!D3Graphics.CalendarGoogle.vars.setted) {
        //Controls    
        $('#back').click(D3Graphics.CalendarGoogle.controls.displayPreviousMonth);
        $('#forward').click(D3Graphics.CalendarGoogle.controls.displayNextMonth);
        D3Graphics.CalendarGoogle.vars.setted = true;
    }*/

    //create svg for the calendar.
    this.base.svg.append("g")
                .attr("transform", "translate(0,0)");

    // Draw rectangles at the appropriate postions, starting from the top left corner. Since we want to leave some room for the heading and buttons,
    // use the gridXTranslation and gridYTranslation variables.
    var cells = this.generate_grid();
    this.base.svg
        .append("g")
        .selectAll(".rect")
        .data(cells)
        .enter()
        .append("rect")
        .attr("x", function (d) { return d[0]; })
        .attr("y", function (d) { return d[1]; })
        .attr("width", this.cell_width())
        .attr("height", this.cell_height())
        .style("stroke", "#555")
        .style("fill", "white");


    // This adds the day of the week headings on top of the grid
    this.base.svg.selectAll("header_days")
        .data([0, 1, 2, 3, 4, 5, 6])
        .enter().append("text")
        .attr("x", function (d) { return cells[d][0]; })
        .attr("y", function (d) { return cells[d][1]; })
        .text(function (d) { return that.days_names[d] });

    // The intial rendering of the dates for the current mont inside each of the cells in the grid. We create a named group ("datesGroup"),
    // and add our dates to this group. This group is also stored globally. Later on, when the the user presses the back and forward buttons
    // to navigate between the months, we clear and re add the new text elements to this group
    D3Graphics.CalendarGoogle.vars.datesGroup = D3Graphics.CalendarGoogle.vars.calendar.append("svg:g");
    var daysInMonthToDisplay = D3Graphics.CalendarGoogle.tools.daysInMonth();

    D3Graphics.CalendarGoogle.vars.datesGroup
        .selectAll("daysText")
        .data(daysInMonthToDisplay)
        .enter()
        .append("text")
        .attr("x", function (d, i) { return cellPositions[i][0]; })
        .attr("y", function (d, i) { return cellPositions[i][1]; })
        .attr("dx", 20) // right padding
        .attr("dy", 20) // vertical alignment : middle
        .attr("transform", "translate(" + D3Graphics.CalendarGoogle.vars.gridXTranslation + "," + D3Graphics.CalendarGoogle.vars.gridYTranslation + ")")
        .text(function (d) { return d[0]; });

    // Create a new svg group to store the chart elements and store it globally. Again, as the user navigates through the months by pressing 
    // the "back" and "forward" buttons on the page, we clear the chart elements from this group and re add them again.
    //D3Graphics.CalendarGoogle.vars.chartsGroup = D3Graphics.CalendarGoogle.vars.calendar.append("svg:g");

    this.render_month();

    // Print de color scale 
    //D3Graphics.CalendarGoogle.tools.drawLegend();
}

D3Graphics.CalendarGoogle.vars = {
    calendarWidthReal: 1100,
    calendarWidth: 950,
    calendarHeight: 850,
    gridXTranslation: 6,
    gridYTranslation: 90,
    
    counter: 0, // Counter is used to keep track of the number of "back" and "forward" button presses and to calculate the month to display.
    currentMonth: new Date().getMonth(),
    monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
    daysOfTheWeek: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
    datesGroup: null,
    calendar: null,
    chartsGroup: null,
    container: '#chart_google',
    data: null,
    setted: false
}

D3Graphics.CalendarGoogle.tools = {
    
    
    

    drawLegend: function () {


        var boxmargin = 4,
            lineheight = 14,
            keyheight = 10,
            keywidth = 40,
            boxwidth = 2 * (keywidth * 1.3),
            formatPercent = d3.format(".0f");

        var sevenshadesofgold = ["#A50026", "#F46D43", "#FEE08B", "#D9EF8B", "#66BD63", "#006837"];

        var title = ['Niveles de', 'rendimiento (kg/ha)'],
            titleheight = title.length * lineheight + boxmargin;

        var x = d3.scale.linear()
            .domain([0, 1]);

        var rendimiento = d3.scale.linear()
                            .domain([0, 1])
                            .range([9000, 11000]);
        //.range([0, 12000]);

        var quantize = d3.scale.quantize()
            .domain([0, 1])
            .range(sevenshadesofgold);

        var ranges = quantize.range().length;

        // return quantize thresholds for the key    
        var qrange = function (max, num) {
            var a = [];
            for (var i = 0; i < num; i++) {
                a.push(i * max / num);
            }
            return a;
        }

        var legend = D3Graphics.CalendarGoogle.vars.calendar.append("svg:g")
            .attr('transform', 'translate(' + D3Graphics.CalendarGoogle.vars.calendarWidth + ',0)')
            .attr("class", "legend-calendar");

        legend.selectAll("text")
            .data(title)
            .enter().append("text")
            .attr("class", "legend-title")
            .attr("y", function (d, i) { return (i + 1) * lineheight - 2; })
            .text(function (d) { return d; })

        var lb = legend.append("rect")
            .attr("transform", "translate (0," + titleheight + ")")
            .attr("class", "legend-box")
            .attr("width", boxwidth)
            .attr("height", ranges * lineheight + 2 * boxmargin + lineheight - keyheight);

        // make quantized key legend items
        var li = legend.append("g")
             .attr("transform", "translate (8," + (titleheight + boxmargin) + ")")
            .attr("class", "legend-items");

        li.selectAll("rect")
            .data(quantize.range().map(function (color) {
                var d = quantize.invertExtent(color);
                if (d[0] == null) d[0] = x.domain()[0];
                if (d[1] == null) d[1] = x.domain()[1];
                return d;
            }))
            .enter().append("rect")
            .attr("y", function (d, i) { return i * lineheight + lineheight - keyheight; })
            .attr("width", keywidth)
            .attr("height", keyheight)
            .style("fill", function (d) { return quantize(d[0]); });

        var round = d3.format(",.0f");

        li.selectAll("text")
            .data(qrange(quantize.domain()[1], ranges))
            .enter().append("text")
            .attr("x", 48)
            .attr("y", function (d, i) { return (i + 1) * lineheight - 2; })
            .text(function (d) { return round(rendimiento(d)); });
    }
}

D3Graphics.CalendarGoogle.controls = {
    displayPreviousMonth: function () {
        // We keep track of user's "back" and "forward" presses in this counter
        D3Graphics.CalendarGoogle.tools.decrementCounter();
        D3Graphics.CalendarGoogle.tools.renderDaysOfMonth();
    },
    displayNextMonth: function () {
        // We keep track of user's "back" and "forward" presses in this counter
        D3Graphics.CalendarGoogle.tools.incrementCounter();
        D3Graphics.CalendarGoogle.tools.renderDaysOfMonth();
    }
}

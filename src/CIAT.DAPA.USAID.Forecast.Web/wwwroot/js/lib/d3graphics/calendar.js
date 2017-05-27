/**
 * This class draws a calendar graphic
 * (Base) base: Configuration to render the graphic
 * (string[]) months_names: Array with name of the months
 * (string[]) days_names: Array with name of the days
 * (object[]) measures: Array with measures
 * (string) measure: Name of measure to display
 * (string) back: Id button with the back function
 * (string) forward: Id button with the forward function
 * (string) label: Id of the title
 * (object[]) ranges: Array with levels of yield standard
 * (string) alias: Alias to call all items inside of the graphic
 */
function Calendar(base, months_names, days_names, measures, measure, back, forward, label, ranges, alias) {
    this.base = base;
    this.counter = 0;
    this.current_month = new Date().getMonth();
    this.months_names = months_names;
    this.days_names = days_names;
    this.measures = measures;
    this.measure = measure;
    this.back = back;
    this.forward = forward;
    this.label = label;
    this.setted = false;
    this.ranges = ranges;
    this.alias = alias;

    this.color_current_month = '#EAEAEA';
    this.color_previous_month = '#FFFFFF';
};

/*
 * Method that return a function to interpolate the values to color
*/
Calendar.prototype.color = function (value) {
    var domain = this.ranges.treashold.map(function (d) { return d + 1; });
    var range = ['#ad5858', '#ad7e58', '#abad58', '#8fad58', '#69ad58'];
    var color = d3.scale.threshold().domain(domain).range(range);
    return color(value);
}

/*
 * Get the width of a cell in the graphic
*/
Calendar.prototype.cell_width = function () { return this.base.width_full / 7; }
/*
 * Get the height of a cell in the graphic
*/
Calendar.prototype.cell_height = function () { return this.base.height_full / 6; }
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
 * Method that return the month to display in the calendar
*/
Calendar.prototype.month_display = function () {
    var dateToDisplay = new Date();
    // We use the counter that keep tracks of "back" and "forward" presses to get the month to display.
    dateToDisplay.setMonth(this.current_month + this.counter);
    return dateToDisplay.getMonth();
}
/*
 * Method that return the year to display in the calendar
*/
Calendar.prototype.year_display = function () {
    var dateToDisplay = new Date();
    // We use the counter that keep tracks of "back" and "forward" presses to get the year to display.
    dateToDisplay.setMonth(this.current_month + this.counter);
    return dateToDisplay.getFullYear();
}

/*
 * Method that filter data from the data source
 * (Date) date: Date of the event
*/
Calendar.prototype.search = function (date) {
    var that = this;
    var filtered = this.base.data.filter(function (item) {
        var date_start = that.base.translate.toDateFromJson(item.start);
        var date_end = that.base.translate.toDateFromJson(item.end);
        return date >= date_start && date <= date_end;
    });
    return filtered.length < 1 ? null : filtered[0];
}

/* 
 * This function generates all the days of the month. But since we have a 7 by 5 grid, we also need to get some of
 * the days from the previous month and the next month. This way our grid will have all its cells filled. The days
 * from the previous or the next month will have a different color though. 
*/
Calendar.prototype.days_month = function () {
    var daysArray = [];

    var firstDayOfTheWeek = new Date(this.year_display(), this.month_display(), 1).getDay();
    var daysInPreviousMonth = new Date(this.year_display(), this.month_display(), 0).getDate();
    // Lets say the first week of the current month is a Wednesday. Then we need to get 3 days from 
    // the end of the previous month. But we can't naively go from 29 - 31. We have to do it properly
    // depending on whether the last month was one that had 31 days, 30 days or 28.
    for (var i = 1; i <= firstDayOfTheWeek; i++)
        daysArray.push([daysInPreviousMonth - firstDayOfTheWeek + i, this.color_current_month]);

    // These are all the days in the current month.
    var daysInMonth = new Date(this.year_display(), this.month_display() + 1, 0).getDate();
    for (i = 1; i <= daysInMonth; i++)
        daysArray.push([i, this.color_previous_month]);

    // Depending on how many days we have so far (from previous month and current), we will need
    // to get some days from next month. We can do this naively though, since all months start on
    // the 1st.
    var daysRequiredFromNextMonth = 35 - daysArray.length;
    for (i = 1; i <= daysRequiredFromNextMonth; i++)
        daysArray.push([i, this.color_current_month]);

    return daysArray.slice(0, 35);
}

/*
 * Method to get data from the current month in the calendar
*/
Calendar.prototype.get_data_month = function () {
    var data = [];
    var date = null;
    var value = null;
    var firstDayOfTheWeek = new Date(this.year_display(), this.month_display(), 1).getDay();
    var daysInPreviousMonth = new Date(this.year_display(), this.month_display(), 0).getDate();
    // Lets say the first week of the current month is a Wednesday. Then we need to get 3 days from 
    // the end of the previous month. But we can't naively go from 29 - 31. We have to do it properly
    // depending on whether the last month was one that had 31 days, 30 days or 28.
    for (var i = 1; i <= firstDayOfTheWeek; i++) {
        date = new Date(this.year_display(), this.month_display() - 1, daysInPreviousMonth - firstDayOfTheWeek + i);
        value = this.search(date);
        data.push(value);
    }

    // These are all the days in the current month.
    var daysInMonth = new Date(this.year_display(), this.month_display(), 0).getDate();
    for (i = 1; i <= daysInMonth; i++) {
        date = new Date(this.year_display(), this.month_display(), i);
        value = this.search(date);
        data.push(value);
    }

    // Depending on how many days we have so far (from previous month and current), we will need
    // to get some days from next month. We can do this naively though, since all months start on
    // the 1st.
    var daysRequiredFromNextMonth = 35 - data.length;
    for (i = 1; i <= daysRequiredFromNextMonth; i++) {
        date = new Date(this.year_display(), this.month_display() + 1, i);
        value = this.search(date);
        data.push(value);
    }
    return data.slice(0, 35);
}
/*
 * Dispatch the event to show next month
*/
Calendar.prototype.next_month = function (e) {
    // We keep track of user's "back" and "forward" presses in this counter
    e.data.instance.counter += 1;
    e.data.instance.render_month();
}

/*
 * Dispatch the event to show prev month
*/
Calendar.prototype.prev_month = function (e) {
    // We keep track of user's "back" and "forward" presses in this counter    
    e.data.instance.counter -= 1;
    e.data.instance.render_month();
}


/*
 * Method that add information to the graphic
*/
Calendar.prototype.render_month = function () {
    var that = this;
    // Show the current month and year
    $(this.label).text(this.months_names[this.month_display()] + ' ' + this.year_display());
    // Get data
    var data = this.get_data_month();

    // We get the days for the month we need to display based on the number of times the user has pressed
    // the forward or backward button.
    var days_to_display = this.days_month();
    var cells = this.generate_grid();

    // Clear the calendar data
    this.base.svg
        .selectAll(".days_month")
        .remove();

    this.base.svg
        .selectAll(".days_yield")
        .remove();

    // All text elements representing the dates in the month are grouped together in the element by the initalizing
    // function below. The initializing function is also responsible for drawing the rectangles that make up the grid.
    this.base.svg
        .append("g")
        .attr("class", "days_month")
        .attr("transform", "translate(0," + that.cell_height() + ")")
        .selectAll("days_month")
        .data(days_to_display)
        .enter()
        .append("text")
        .attr("x", function (d, i) { return cells[i][0]; })
        .attr("y", function (d, i) { return cells[i][1]; })
        .attr("dx", 10) // right padding
        .attr("dy", 15) // vertical alignment 
        .text(function (d) { return d[0]; }); // Render text for the day of the week

    var data_month = this.get_data_month();

    this.base.svg
            .append("g")
            .attr("class", "days_yield")
            .attr("transform", "translate(0," + that.cell_height() + ")")
            .selectAll("days_yield")
            .data(data_month)
            .enter()
            .append("text")
            .attr("x", function (d, i) { return cells[i][0]; })
            .attr("y", function (d, i) { return cells[i][1]; })
            .attr("dx", this.cell_width() / 4) // right padding
            .attr("dy", 2 * (this.cell_height() / 3)) // vertical alignment 
            .text(function (d) {
                var text = '';
                if (d != null)
                    text = that.base.formats.round(d.data.filter(function (item) { return item.measure === that.measure; })[0].avg);
                return text;
            }); // Render text for the day of the week

    // Paint the cells 
    this.base.svg.selectAll(".calendar_days rect")
            .data(days_to_display)
            // Here we change the color depending on whether the day is in the current month, the previous month or the next month.
            // The function that generates the dates for any given month will also specify the colors for days that are not part of the
            // current month. We just have to use it to fill the rectangle
            .style("fill", function (d, i) {
                var bg = '';
                if (d[1].indexOf('FFFFFF')) {
                    if (data_month[i] != null)
                        bg = that.color(data_month[i].data.filter(function (item) { return item.measure === that.measure; })[0].avg);
                    else
                        bg = d[1];
                }
                else
                    bg += d[1];
                return bg;
            })
            .on("mouseover", function (d, i) {
                if (data_month[i] != null) {
                    var measures_names = data_month[i].data.map(function (item) { return item.measure; });
                    var content_rows = '';
                    for (var j = 0; j < measures_names.length; j++) {
                        var m = that.measures.filter(function (item) { return item.name === measures_names[j]; })[0];
                        if(m != undefined)
                            content_rows += '<tr><td>' + m.label + '</td><td>' + data_month[i].data.filter(function (item) { return item.measure === measures_names[j]; })[0].avg.toFixed(0) + ' ' + m.metric + '</td></tr>';
                    }
                    that.base.tooltip_show(d3.event.pageX + 20, d3.event.pageY, '<table class="table">' + content_rows + '</table>');
                }
            })
            .on("mouseout", function (d) {
                that.base.tooltip_hide();
            });

},

/*
 * Method that render the graphic in a container
*/
Calendar.prototype.render = function () {
    var that = this;
    that.base.init(true, 0.5);

    if (!this.setted) {
        //Controls    
        $(this.back).click({ instance: this }, this.prev_month);
        $(this.forward).click({ instance: this }, this.next_month);
        this.setted = true;
    }

    // Create the partition of the graphic in cells
    // The first row is for the header
    var cells = this.generate_grid();

    // Add legend colors  
    var array_labels = Array.apply(null, { length: that.ranges.labels.length }).map(Number.call, Number);
    this.base.svg
        .append("g")
        .attr("class", "legend_colors")
        .selectAll("legend_colors")
        .data(array_labels)
        .enter()
        .append("rect")
        .attr("x", function (d) { return cells[d][0]; })
        .attr("y", 0)
        .attr("width", this.cell_width())
        .attr("height", (that.cell_height() / 2))
        .style("fill", function (d) {
            return that.color(that.ranges.treashold[d]);
        });

    // Add legend labels
    this.base.svg
        .append("g")
        .attr("class", "legend_labels")
        .selectAll("legend_labels")
        .data(array_labels)
        .enter()
        .append("text")
        .attr("x", function (d) { return cells[d][0]; })
        .attr("y", 20)
        .text(function (d) { return that.ranges.labels[d]; });

    // This adds the day of the week headings on top of the grid
    this.base.svg
        .append("g")
        .attr("class", "header_days")
        .selectAll("header_days")
        .data([0, 1, 2, 3, 4, 5, 6])
        .enter()
        .append("text")
        .attr("x", function (d) { return cells[d][0]; })
        .attr("y", "0")
        .attr("dy", that.cell_height() - 5)
        .text(function (d) { return that.days_names[d]; });

    // Draw rectangles at the appropriate postions, starting from the top left corner. Since we want to leave some room for the heading and buttons
    this.base.svg
        .append("g")
        .attr("class", "calendar_days")
        .attr("transform", "translate(0," + that.cell_height() + ")")
        .selectAll("rect")
        .data(cells)
        .enter()
        .append("rect")
        .attr("x", function (d) { return d[0]; })
        .attr("y", function (d) { return d[1]; })
        .attr("width", this.cell_width())
        .attr("height", this.cell_height());

    // Render the current month
    this.render_month();

}
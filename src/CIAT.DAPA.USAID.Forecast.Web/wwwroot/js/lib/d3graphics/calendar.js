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
    /*var domain = this.ranges.treashold.map(function (d) { return d + 1; });
    var range = ['#ad5858', '#ad7e58', '#abad58', '#8fad58', '#69ad58'];
    var color = d3.scale.threshold().domain(domain).range(range);
    return color(value);*/
    var c = this.ranges.filter(function (item2) { return item2.lower < value && item2.upper >= value; })[0];
    console.log(c);
    console.log(this.ranges);
    console.log(value);
    return value;
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
        .attr("dx", 1) // right padding
        .attr("dy", 10) // vertical alignment 
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
                    if (m != undefined)
                        content_rows += '<tr><td>' + m.label + '</td><td>' + data_month[i].data.filter(function (item) { return item.measure === measures_names[j]; })[0].avg.toFixed(0) + ' ' + m.metric + '</td></tr>';
                }
                that.base.tooltip_show(d3.event.pageX + 20, d3.event.pageY, '<table class="table"><tr><th colspan="2">Valor promedio de predicción<th><tr>' + content_rows + '</table>');
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
    var array_labels = Array.apply(null, { length: that.ranges.length }).map(Number.call, Number);
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
            return that.color(that.ranges[d].color);
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
        .text(function (d) { return that.ranges[d].name; });

        

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


/**
 * This class contains all settings and some tools for the graphics
 * (string) container: Name of the div where the graphic will display
 * (object) data: Set of information
 */
function Base(container, data) {

    // Name of the div where the graphic will display
    this.container = container;
    // Data of the graphic
    this.data = data;

    // Width of container    
    this.width_full = 0;
    // Width whit margin
    this.width = 0;
    // Height of container    
    this.height_full = 0;
    // Height whit margin
    this.height = 0;
    // Margin
    this.margin = {
        top: 0,
        right: 0,
        left: 0,
        bottom: 0
    };

    // SVG 
    this.svg = null;

    // Animation settings
    this.animation = {
        duration: 1500,
        delay: 500
    }

    // Formats tho show the information or values in the graphics
    this.formats = {
        round: d3.format(",.0f"),
        float: d3.format(",.1f"),
        date: d3.time.format('%Y-%m-%d').parse,
        date_format: d3.time.format("%Y-%m-%d")
    };

    // Tools to translate data from one format to javascript objects
    this.translate = {
        // translate from c# datetime to Date javascript
        toDateFromJson: function (date) {
            return new Date(date.substring(0, 4), parseInt(date.substring(5, 7)) - 1, date.substring(8, 10));
        }
    };

    // Tooltip
    this.tooltip = null;

    // Date data
    this.month_names = [];
    this.days_names = [];

    // Class CSS
    this.class = '';
    this.classes = [];

    // labels axis
    this.axis_labels = {
        y: '',
        x: ''
    };

}

/*
 * Method that set the value of the y axis label
 * (string) y: String with the label
*/
Base.prototype.setAxisLabelY = function (y) {
    this.axis_labels.y = y;
}

/*
 * Method that set the class for the graphic
 * (string) class_name: String with the name of the class
*/
Base.prototype.setClass = function (class_name) {
    this.class = class_name;
}

/*
 * Method that set the classes for the graphic
 * (string[]) list: Array of String with the names of the classes
*/
Base.prototype.setClasses = function (list) {
    this.classes = list;
}

/*
 * Method that set the date date
 * (string []) months: Array with month names
 * (string []) days: Array with days names
*/
Base.prototype.setDateNames = function (months, days) {
    this.month_names = months;
    this.days_names = days;
}

/*
 * Method that set the same size for the margin of the graphic
 * (int) size: Pixel of margin
*/
Base.prototype.setMargin = function (size) {
    this.margin.top = size;
    this.margin.right = size;
    this.margin.left = size;
    this.margin.bottom = size;
}

/*
 * Method that set the same size for the margin of the graphic
 * (int) top: Margin top
 * (int) right: Margin right
 * (int) left: Margin left
 * (int) bottom: Margin bottom
*/
Base.prototype.setMargin = function (top, right, left, bottom) {
    this.margin.top = top;
    this.margin.right = right;
    this.margin.left = left;
    this.margin.bottom = bottom;

}

/*
 * Method that get full margin veritcal of the graphic
*/
Base.prototype.getMarginVertical = function () {
    return (this.margin.top + this.margin.bottom);
}

/**
 * This method set the init values
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
 */
Base.prototype.init = function (relative, height) {
    $(this.container).html('');
    this.svg = d3.select(this.container).append("svg");
    this.tooltip = d3.select("body").append("div")
        .attr("class", "tooltip")
        .style("opacity", 0);
    this.update(relative, height);
}

/*
 * Method that update the graphic's dimensions
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
*/
Base.prototype.update = function (relative, height) {
    var element = document.getElementById(this.container.replace('#', ''));
    this.width_full = element.clientWidth;
    this.height_full = relative == true ? this.width_full * height : height;
    this.width = this.width_full - (this.margin.left + this.margin.right);
    this.height = this.height_full - (this.margin.top + this.margin.bottom);
    this.svg.attr('width', this.width_full)
        .attr('height', this.height_full);
}

/*
 * Method that create a scale for y axis
 * (function) y: Function to scale the y axis
 * (int) ticks: Count of splitters
*/
Base.prototype.getYAxis = function (y, ticks) {
    var t = $(window).width() > 767 ? ticks : ticks / 2;
    return d3.svg.axis().scale(y).ticks(t).orient("left");
}

/*
 * Method that create a scale for x axis
 * (function) x: Function to scale the x axis
*/
Base.prototype.getXAxis = function (x) {
    return d3.svg.axis().scale(x).orient("bottom");
}

/*
 * Method to add labels in the axis
 * (function) x: Function to interpolate the x values
 * (function) y: Function to interpolate the y values
*/
Base.prototype.addAxisLabels = function (x, y) {
    // Add y label axis
    if (this.axis_labels.y !== '')
        this.svg.append('g')
            .attr('class', 'y_axis_label')
            .call(y)
            .append('text')
            .attr('transform', 'rotate(-90)')
            .attr('x', -(this.height / 2))
            .attr('y', '10')
            .style('text-anchor', 'middle')
            .text(this.axis_labels.y);

    if (this.axis_labels.x !== '')
        this.svg.append('g')
            .attr('class', 'y_axis_label')
            .call(x)
            .append('text')
            .attr("transform", 'translate(0,' + (this.height + this.margin.bottom) + ')')
            .attr('x', (this.width / 2))
            .style('text-anchor', 'middle')
            .text(this.axis_labels.x);
}

/*
 * Method that add the axis x and y in the graphic
 * (function) x: Function to interpolate the x values
 * (function) y: Function to interpolate the y values
 * (int) ticks: Count of ticks
*/
Base.prototype.addAxis = function (x, y, ticks) {
    // Add x-axis to the histogram svg.
    this.svg.append("g").attr("class", "x_axis")
        .attr("transform", 'translate(0,' + this.height + ')')
        .call(this.getXAxis(x));

    // Add y-axis to the histogram svg.
    this.svg.append("g")
        .attr("class", "y_axis")
        .attr("transform", "translate(" + this.margin.right + ",0)")
        .call(this.getYAxis(y, ticks));

    this.addAxisLabels(x, y);
}

/*
 * Method that add the axis x and y in the graphic
 * (function) x: Function to interpolate the x values
 * (function) y: Function to interpolate the y values
 * (int) ticks: Count of ticks
 * (int) grade: Grades to rotate the x axis
*/
Base.prototype.addAxisDate = function (x, y, ticks, grade) {
    // Add x-axis to the histogram svg.
    this.svg.append("g").attr("class", "x_axis")
        .attr("transform", 'translate(0,' + this.height + ')')
        .call(d3.svg.axis().scale(x).tickFormat(d3.time.format('%b %d')));

    // Add y-axis to the histogram svg.
    this.svg.append("g")
        .attr("class", "y_axis")
        .attr("transform", "translate(" + this.margin.right + ",0)")
        .call(this.getYAxis(y, ticks));

    this.addAxisLabels(x, y);
}

/*
 * Method that add the axis x and y in the graphic
 * (function) x: Function to interpolate the x values
 * (function) y: Function to interpolate the y values
 * (int) ticks: Count of ticks
 * (int) grade: Grades to rotate the x axis
 * (string) x_text: Text in x axis
 * (string) y_text: Grades to rotate the x axis
*/
Base.prototype.addAxisDateText = function (x, y, ticks, grade, x_text, y_text) {
    // Add x-axis to the histogram svg.
    this.svg.append("g").attr("class", "x_axis")
        .attr("transform", 'translate(0,' + (this.height - 10) + ')')
        .call(d3.svg.axis().scale(x).tickFormat(d3.time.format('%b %d')));

    // Add y-axis to the histogram svg.
    this.svg.append("g")
        .attr("class", "y_axis")
        .attr("transform", "translate(" + this.margin.right + ",0)")
        .call(this.getYAxis(y, ticks));

    this.axis_labels.y = y_text;
    this.axis_labels.x = x_text;
    this.addAxisLabels(x, y);
}

/*
 * Method that create a function for the ticks
 * (function) xy: X or Y function to create interpolation
 * (int) ticks: Count of ticks
 * (double) size: Size of the ticks
*/
Base.prototype.getTicks = function (xy, ticks, size) {
    return d3.svg.axis().scale(xy)
        .ticks(ticks)
        .tickSize(size)
        .tickFormat('');
}

/*
 * Method that add lines ticks in the graphic
 * (function) x: Function to interpolate the x axis
 * (function) y: Function to interpolate the y axis
 * (int) x_ticks: Count of the ticks in x axis
 * (int) y_ticks: Count of the ticks in y axis
*/
Base.prototype.addAxisTicks = function (x, y, x_ticks, y_ticks) {
    this.svg.append('g')
        .attr('class', 'x_axis_ticks')
        .attr('transform', 'translate(0,' + this.height + ')')
        .call(this.getTicks(x, x_ticks, -this.height));

    this.svg.append('g')
        .attr('class', 'y_axis_ticks')
        .attr('transform', 'translate(' + this.margin.right + ',0)')
        .call(this.getTicks(y, y_ticks, this.width).orient('right'));
}

/*
 * Method that add legend to the graphic
 * (string) location: Place where you want to put the legend (bottom, right)
 * (object) content: Information that should the legend has ({title,value,class})
*/
Base.prototype.addLegend = function (location, content) {
    var that = this;

    // Create function for y-axis map.
    var y_legend = d3.scale.ordinal()
        .domain(content.map(function (item) { return item.title; }))
        .rangePoints([0, 15 * content.length]);
    var legend = null;

    // Select the location of the legend
    if (location === 'bottom') {
        this.svg.attr('height', this.height_full + (20 * content.length));
        legend = this.svg.append('g');
        legend.attr('transform', 'translate(0,' + this.height + ')');
    }
    else {
        this.svg.attr('width', this.width_full * 1.3);
        legend = this.svg.append('g');
        legend.attr('transform', 'translate(' + this.width + ',0)');
    }

    // Add rect with color of every item
    legend.selectAll(".legend")
        .data(content)
        .enter()
        .append("rect")
        .attr("x", '0')
        .attr("y", function (d) { return y_legend(d.title) - 15; })
        .attr("width", '20')
        .attr("height", '20')
        .attr('class', function (d) { return d.class; });
    // Add text with titles of the legend
    legend.selectAll(".legend")
        .data(content)
        .enter()
        .append('text')
        .attr("x", '22')
        .attr("y", function (d) { return y_legend(d.title); })
        .text(function (d) { return d.value + ' - ' + d.title; });
}

/*
 * Method that show a tooltip in the graphic
 * (int) left: Distance of the side left in the web page
 * (int) top: Distance of the side top in the web page
 * (string) content: Content of the tooltip
*/
Base.prototype.tooltip_show = function (left, top, content) {
    this.tooltip.transition()
        .duration(200)
        .style("opacity", .9);
    this.tooltip.html(content)
        .style("left", left + "px")
        .style("top", top + "px");
}

/*
 * Method that hide the tooltip of the graphic
*/
Base.prototype.tooltip_hide = function () {
    this.tooltip.transition()
        .duration(500)
        .style("opacity", 0);
}
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
            .attr('x', (this.width/2))
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
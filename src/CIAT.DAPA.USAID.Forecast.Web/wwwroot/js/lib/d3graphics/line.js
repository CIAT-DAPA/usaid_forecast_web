/**
 * This class draws a line graphic
 * (Base) base: Configuration to render the graphic
 */
function Line(base) {
    this.base = base;

    // circleContainer;
}

/*
 * Method to interpolate
*/
Line.prototype.tween= function (b, callback) {
    return function (a) {
        var i = d3.interpolateArray(a, b);

        return function (t) {
            return callback(i(t));
        };
    };
}

Line.prototype.render = function (data) {    
    var that = this;

    this.base.init(true, 0.5);
    
    // TODO code duplication check how you can avoid that
    var x = d3.time.scale().range([this.base.margin.right, this.base.width]);
    var y = d3.scale.linear().range([this.base.height, 0]);

    var area = d3.svg.area()
            .interpolate('linear')
            .x(function (d) { return x(d.date) + that.base.margin.right / 2; })
            .y0(this.base.height)
            .y1(function (d) { return y(d.value); });

    var line = d3.svg.line()
            .interpolate('linear')
            .x(function (d) { return x(d.date) + that.base.margin.right / 2; })
            .y(function (d) { return y(d.value); });

    var startData = data.map(function (datum) {
        return {
            date: datum.date,
            value: 0
        };
    });

    // Compute the minimum and maximum date, and the maximum value.
    x.domain([data[0].date, data[data.length - 1].date]);
    // Compute the maximun value more 10%
    y.domain([0, d3.max(data, function (d) { return d.value; }) * 1.1]);

    // Add the axis
    this.base.addAxis(x, y, 12);

    // Add the ticks
    this.base.addAxisTicks(x, y, data.length, 12 );

    // Add the line path.
    this.base.svg.append('path')
            .datum(startData)
            .attr('class', 'line_area_line')
            .attr('d', line)
            .transition()
            .duration(that.base.animation.duration)
            .delay(that.base.animation.delay)
            .attrTween('d', that.tween(data, line))
            .each('end', function () {
                //D3Graphics.Line.tools.drawCircles(data, svg, x, y, detailWidth, detailHeight, detailMargin, circleContainer);
            });

    // Add the area path.
    this.base.svg.append('path')
        .datum(startData)
        .attr('class', 'line_area')
        .attr('d', area)
        .transition()
        .duration(that.base.animation.duration)
        .attrTween('d', that.tween(data, area));

    // Line Normal
    
    /*this.base.svg.append('line')
        .attr('x1', 0)
        .attr('y1', y(124))
        .attr('x2', this.base.width)
        .attr('y2', y(124))
        .attr("stroke-width", 2)
        .attr("stroke", "red");*/
}

/*
D3Graphics.Line.tools = {
    drawCircle: function (datum, index, x, y, detailWidth, detailHeight, detailMargin, circleContainer) {

        circleContainer.datum(datum)
            .append('circle')
            .attr('class', 'lineChart--circle')
            .attr('r', 0)
            .attr('cx', function (d) { return x(d.date) + detailWidth / 2; })
            .attr('cy', function (d) { return y(d.value); })
            .on('mouseenter', function (d) {
                d3.select(this)
                    .attr('class', 'lineChart--circle lineChart--circle__highlighted')
                    .attr('r', 7);
                d.active = true;
                D3Graphics.Line.tools.showCircleDetail(d, x, y, detailHeight, detailMargin, detailWidth, circleContainer);
            })
            .on('mouseout', function (d) {
                d3.select(this)
                    .attr('class', 'lineChart--circle')
                    .attr('r', 6);
                if (d.active) {
                    D3Graphics.Line.tools.hideCircleDetails(circleContainer);
                    d.active = false;
                }
            })
            .on('click touch', function (d) {
                if (d.active) {
                    D3Graphics.Line.tools.showCircleDetail(d, x, y, detailHeight, detailMargin, detailWidth, circleContainer);
                } else {
                    D3Graphics.Line.tools.hideCircleDetails(circleContainer);
                }
            })
            .transition()
            .delay(D3Graphics.Line.vars.DURATION / 10 * index)
            .attr('r', 6);
    },
    drawCircles: function (data, svg, x, y, detailWidth, detailHeight, detailMargin, circleContainer) {
        circleContainer = svg.append('g');

        data.forEach(function (datum, index) {
            D3Graphics.Line.tools.drawCircle(datum, index, x, y, detailWidth, detailHeight, detailMargin, circleContainer);
        });
    },
    hideCircleDetails: function (circleContainer) {
        circleContainer.selectAll('.lineChart--bubble')
            .remove();
    },

    showCircleDetail: function (data, x, y, detailHeight, detailMargin, detailWidth, circleContainer) {
        var details = circleContainer.append('g')
            .attr('class', 'lineChart--bubble')
            .attr(
            'transform',
            function () {
                var result = 'translate(';

                result += x(data.date);
                result += ', ';
                result += y(data.value) - detailHeight - detailMargin;
                result += ')';

                return result;
            }
            );

        details.append('path')
            .attr('d', 'M2.99990186,0 C1.34310181,0 0,1.34216977 0,2.99898218 L0,47.6680579 C0,49.32435 1.34136094,50.6670401 3.00074875,50.6670401 L44.4095996,50.6670401 C48.9775098,54.3898926 44.4672607,50.6057129 49,54.46875 C53.4190918,50.6962891 49.0050244,54.4362793 53.501875,50.6670401 L94.9943116,50.6670401 C96.6543075,50.6670401 98,49.3248703 98,47.6680579 L98,2.99898218 C98,1.34269006 96.651936,0 95.0000981,0 L2.99990186,0 Z M2.99990186,0')
            .attr('width', detailWidth)
            .attr('height', detailHeight);

        var text = details.append('text')
            .attr('class', 'lineChart--bubble--text');

        text.append('tspan')
            .attr('class', 'lineChart--bubble--label')
            .attr('x', detailWidth / 2)
            .attr('y', detailHeight / 3)
            .attr('text-anchor', 'middle')
            .text(data.label);

        text.append('tspan')
            .attr('class', 'lineChart--bubble--value')
            .attr('x', detailWidth / 2)
            .attr('y', detailHeight / 4 * 3)
            .attr('text-anchor', 'middle')
            .text(data.value);
    },
    tween: function (b, callback) {
        return function (a) {
            var i = d3.interpolateArray(a, b);

            return function (t) {
                return callback(i(t));
            };
        };
    }
}
*/
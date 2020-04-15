/**
 * Method that plot the ranges in a table
 * @param {any} data
 * @param {any} control
 */
function plot_ranges(data, control) {
    var text = '<tr>';
    for (var i = 0; i < data.length; i++) {
        text = text +
            '<td bgcolor="' + data[i].color + '">' +
            data[i].label + ' ' + (i == 0 ? data[i].upper + '<' : (i == (data.length - 1) ? '>' + data[i].lower : '[' + data[i].lower + '-' + data[i].upper + ']'))+
            '</td>';
    }
    text = text + '</tr>';
    $("#" + control).html(text);
}

/**
 * Method that merge ranges with colors
 * @param {any} ranges
 * @param {any} colors
 */
function merge_ranges_colors(ranges, colors) {
    var t_ranges = []
    for (var i = 0; i < ranges.length; i++) {
        t_ranges.push(ranges[i]);
        t_ranges[i].color = colors[i];
    }
    return t_ranges;
}
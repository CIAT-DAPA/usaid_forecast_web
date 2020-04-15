/**
 * Method that get the value of the parameter from url
 * @param {any} pos
 */
function search(pos) {
    var url = window.location.href;
    var parameters = url.split("/");
    return parameters.length > (pos + 3) ? decodeURIComponent(parameters[pos + 3]) : null;
}

/**
 * Method to change the background image of the website
 * */
function updateBackground() {
    var bg = '';
    if (window.location.href.includes('/Cultivo') || window.location.href.includes('/cultivo')) {
        var para = search(4).toLowerCase();
        bg = (para === 'arroz' ? 'bg-rice' : 'bg-maize');
    }
    else
        bg = 'bg-climate';
    $(".body-content").addClass(bg);
}
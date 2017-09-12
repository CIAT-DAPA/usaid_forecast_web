'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.SetupFactory
 * @description
 * # SetupFactory
 * Setup in the ForecastApp.
 */
angular.module('ForecastApp')    
    .factory('tools', function (config) {
        var dataFactory = {};

        /*
         * Method that get the value of the parameter from url
         * (string) name: Parameter name
        */
        dataFactory.search = function (name) {
            var url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }

        /*
         * Method to determinate which is the current webpage
        */
        dataFactory.source = function () {
            if (window.location.href.includes('/Clima') || window.location.href.includes('/clima'))
                return 'climate';
            else if (window.location.href.includes('/Cultivo') || window.location.href.includes('/cultivo'))
                return 'crop';
            else if (window.location.href.includes('/Experto') || window.location.href.includes('/experto'))
                return 'expert';
            else if (window.location.href.includes('/Home/Glosario') || window.location.href.includes('/Home/Glosario'))
                return 'glossary';
            else
                return 'about';
        }

        /*
         * Method that changes the content of the modal window
         * (string) title: Title of the modal window
         * (string) text: Content in the modal window
         * (string) url: Url to see a video, if it is null or empty, the video won't show
        */
        dataFactory.show_assist = function (title, text, url) {
            $("#modal_title").html(title);
            $("#modal_text").html(text);
            if (url != null && url !== '')
                $("#modal_video").attr("src", url);
            else
                $("#modal_video").remove();
        }

        /*
         * Method that return a date in spanish text
         * (string) date: Date in format yyyy-MM-nn
        */
        dataFactory.dateToText = function (date) {
            var d = new Date(date);
            return d.getDate()  + " de " + config.month_names[d.getMonth()] + " de " + d.getFullYear();
        }

        /*
         * Method to change the background image of the website
        */
        dataFactory.updateBackground = function () {
            var bg = '';            
            if (window.location.href.includes('/Cultivo') || window.location.href.includes('/cultivo')) {
                var para = dataFactory.search('cultivo').toLowerCase();
                bg = (para === 'arroz' ? 'bg-rice' : 'bg-maize');
            }
            else
                bg = 'bg-climate';
            $(".body-content").addClass(bg);
        }

        return dataFactory;
    })
    .factory('setup', ['config', function (config) {
        var dataFactory = {};

        /*
         * Method that return all options by section of the sub menu
         * (string) section: Name of section
        */
        dataFactory.listSubMenuOption = function (section) {
            return config.sub_menu[section];
        }

        /*
         * Method that return all climate vars avaliable
        */
        dataFactory.getClimateVars = function () {
            return config.climate_vars;
        }

        /*
         * Method that return all climate vars avaliable
        */
        dataFactory.getClimateVarsScenario = function () {
            return config.climate_vars_scenario;
        }

        /*
         * Method that return the climate vars for the forecast
        */
        dataFactory.getClimatologyVarsForecast = function () {
            return config.climatology_forecast;
        }

        /*
         * Method that return the float fixed
        */
        dataFactory.getFloat = function () {
            return config.float;
        }

        /*
         * Method that return all days in spanish
        */
        dataFactory.getDays = function () {
            return config.days_names;
        }

        /*
        * Method that return all days in spanish
       */
        dataFactory.getDaysCalendar = function () {
            return config.days_names_calendar;
        }

        /*
         * Method that return all months in spanish
        */
        dataFactory.getMonths = function () {
            return config.month_names;
        }

        /*
         * Method that return all months in spanish with index
        */
        dataFactory.getMonthsFull = function () {
            return config.month_names.map(function (item, i) { return { id: i+1, name: item }; });
        }

        /*
         * Method that return all database to download data
        */
        dataFactory.getExpertDatabases = function () {
            return config.expert_db;
        }

        return dataFactory;
    }]);
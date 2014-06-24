$(document).ready(function () {
    $('#build-widget').teamCityWidget();
});

$.fn.teamCityWidget = function() {
    return this.each(function() {        
        var container = $(this);
        var buildType = container.data('buildType');
        
        container.empty();
        
        var buildMessage = function(title, body) {
            var msg = $('<div class="alert alert-block alert-warning no-release">');
            msg.append('<button type="button" class="close" data-dismiss="alert">×</button>');
            msg.append('<h4 class="alert-heading">' + title + '</h4>');
            msg.append('<p>' + body + '</p>');
            
            return msg;
        };
        
        $('#status-widget').find('.alert').remove();

        var noReleaseBuild = function() {
            buildMessage('Not public yet', 'This project is currently only available on our edge feed or there is a problem communicating with the build server.').insertBefore('#community');
        };
        
        var renderBuild = function(build) {
            var item = $('<li></li>');
            
            item.append('<button class="btn ' + (build.success ? 'build-success' : 'build-failure') + '" type="button">' + build.number + '</button>');
            item.append('<label class="label">' + build.type + '</label>');
            
            item.appendTo(container);
        };

        // TODO -- bring these back when we get TC configured
        //$.teamCity.api.forLatestBuild(buildType, renderBuild, function() {
        //    // maybe hide the build status text if we can't find a build
        //});
        //$.teamCity.api.forLatestRelease(buildType, renderBuild, noReleaseBuild);

        noReleaseBuild();
    });
};

(function($) {
    
    if (typeof String.prototype.endsWith !== 'function') {
        String.prototype.endsWith = function(suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }
    
    function TeamCityService(baseUrl) {
        if(baseUrl.endsWith('/')) {
            baseUrl = baseUrl.substr(0, baseUrl.length - 1);
        }
        
        this.baseUrl = baseUrl;
        this.restUrl = baseUrl + '/httpAuth/app/rest';
    }
    
    TeamCityService.prototype = {
        forLatestBuild: function(buildType, continuation, none) {
            this.queryBuild('/builds/buildType:' + buildType + ',lookupLimit:1', 'Latest', continuation, none);
        },
        forLatestRelease: function(buildType, continuation, none) {
            this.queryBuild('/builds/?locator=buildType:' + buildType + ',tags:release', 'Release', continuation, none);
        },
        queryBuild: function(url, type, continuation, none) {
            if(typeof(none) != 'function') {
                none = function() { };
            }
            
            $.ajax({
                url: this.restUrl + url,
                type: 'GET',
                dataType: 'json'
            })
            .done(function(response, status, xhr) {
                if(status != "success" || !response) {
                    none();
                    return;
                }
                
                data = response;
                if(typeof(response['count']) != 'undefined') {
                    if(response.count == 0) {
                        none();
                        return;
                    }
                    
                    data = response.build[0];
                }
                
                var build = {
                    id: data.id,
                    type: type,
                    number: data.number,
                    success: data.status == "SUCCESS"
                };
                
                continuation(build);
            })
            .fail(function() { none(); });
        }
    };
    
    $.extend(true, $, {
        'teamCity': {
            'api': new TeamCityService('http://build.fubu-project.org')
        }
    });
    
}(jQuery));
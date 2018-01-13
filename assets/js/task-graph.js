$(function () {
    $.ajax({
        dataType: "json",
        url: "tasks/tasklist.json",
        success: function(data) {
            $.each(data, function (i, item) {
                $("#selectTask").append($("<option>", { 
                    value: item,
                    text : item 
                }));
            });

            $("select").first().trigger("change");
        }
    });

    var selectTask = $("#selectTask");
    $("select").on("change", function() {
        var taskName = selectTask.val();
        var layoutType = "breadthfirst";
        $.ajax({
            dataType: "json",
            url: "tasks/" + taskName + ".json",
            success: function(data) {
                var cy = cytoscape({
                    container: document.getElementById("task-graph"),
                    elements: data,
                    layout: {
                        name: layoutType,
                        directed: true,
                        padding: 10
                    },
                    style: cytoscape.stylesheet()
                        .selector("node")
                        .css({
                            'content': "data(id)",
                            'text-valign': "center",
                            'color': "white",
                            'text-outline-width': 2,
                            'text-outline-color': "black",
                            'background-color': "black",
                            'font-size': "36px",
                            'height': "50px",
                            'width': "50px"
                        })
                        .selector("edge")
                        .css({
                            'curve-style': "bezier",
                            'source-arrow-shape': "triangle",
                            'control-point-distance': 0
                        }),
                    //zoomingEnabled: false,
                    userZoomingEnabled: false,
                    // panningEnabled: false,
                    userPanningEnabled: false,
                    boxSelectionEnabled: false
                });
            }
        });
    }); 
});
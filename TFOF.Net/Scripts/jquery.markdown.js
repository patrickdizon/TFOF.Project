$(function() {
   $(".markdown-button").click(function() {
       var container = $(this).parent().parent();
       if ($(this).hasClass('toggled')) {
           $(this).html('Preview')
           $(this).removeClass('toggled');
           container.find(".markdown-preview").hide();
           container.find("textarea").show();
       } else {
           $(this).addClass('toggled');
           $(this).html('Edit')
           $.get("/markdown/", {text: container.find("textarea").val()}, function(res) {
               container.find(".markdown-preview").html(res);
               container.find(".markdown-preview").show();
               container.find("textarea").hide();
           });
       }
   });
});
(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var logo = document.getElementsByClassName("link");
            logo[0].href = "https://www.endava.com/en/Industries/Healthtech";
            logo[0].target = "_blank";
            logo[0].children[0].alt = "Implemeting Swagger";
            logo[0].children[0].src = "/swagger/endava-logo.png";
        });
    });
})();
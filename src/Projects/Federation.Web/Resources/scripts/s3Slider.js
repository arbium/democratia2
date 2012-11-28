/* ------------------------------------------------------------------------
s3Slider
	
Developped By: Boban-> http://www.serie3.info/
CSS Help: -> http://www.perspectived.com/
Version: 1.0
	
Copyright: Feel free to redistribute the script/modify it, as
long as you leave my infos at the top.
------------------------------------------------------------------------- */


(function ($) {

    $.fn.s3Slider = function (vars) {
        /*        window.alert(this[0])*/
        var element = this;
        var timeOut = (vars.timeOut != undefined) ? vars.timeOut : 40000;
        var current = null;
        var timeOutFn = null;
        var faderStat = true;
        var mOver = false;
        var items = $("." + element[0].className + "Content ." + element[0].className + "Image");
        var itemsSpan = $("." + element[0].className + "Content ." + element[0].className + "Image span");

        items.each(function (i) {

            $(items[i]).mouseover(function () {
                mOver = true;
            });

            $(items[i]).mouseout(function () {
                mOver = false;
                fadeElement(true);
            });

        });

        var fadeElement = function (isMouseOut) {
            var thisTimeOut = (isMouseOut) ? (timeOut / 2) : timeOut;
            thisTimeOut = (faderStat) ? 10 : thisTimeOut;
            if (items.length > 0) {
                timeOutFn = setTimeout(makeSlider, thisTimeOut);
            } else {
                console.log("Poof..");
            }
        }

        var makeSlider = function () {
            if (items.length == 1) { return; }
            //current = (current != null) ? current : items[(items.length-1)];
            current = (current != null) ? current : items[(items.length)];
            var currNo = jQuery.inArray(current, items);
            if (currNo >= items.length || (currNo <= 0)) { currNo = 0; }

            var newMargin = $(element).width() * currNo;
            if (faderStat == true) {
                if (!mOver) {
		    var newwidth = $(".main_visual").width();
		    $(items[currNo]).find("img").css({'width':newwidth});

			/*
		    var newheight = $(items[currNo]).find("img").height();
		    if ( newheight == 0) {
			$(items[currNo]).css('display','block');
			var newheight = $(items[currNo]).find("img").height();
			$(".main_visual").height(newheight);

			//var newheight = $(items[currNo]).find("img").height();
			//$(".main_visual").animate({ 'height': newheight}, (timeOut / 6), function() {  
				// Animation complete. 
			//});
			$(".s3slider").height(newheight);

		    } */


                    $(items[currNo]).fadeIn((timeOut / 6), function () {

			var newheight = $(items[currNo]).find("img").height();
			$(".main_visual").animate({ 'height': newheight}, (timeOut / 6), function() {  
				// Animation complete. 
			});
			$(".s3slider").height(newheight);


                        if ($(itemsSpan[currNo]).css('bottom') == 0) {
                            $(itemsSpan[currNo]).slideUp((timeOut / 6), function () {
                                faderStat = false;
                                current = items[currNo];
                                if (!mOver) {
                                    fadeElement(false);
                                }
                            });
                        } else {
                            $(itemsSpan[currNo]).slideDown((timeOut / 6), function () {
                                faderStat = false;
                                current = items[currNo];
                                if (!mOver) {
                                    fadeElement(false);
                                }
                            });
                        }
                    });
                }
            } else {
                if (!mOver) {
                    if ($(itemsSpan[currNo]).css('bottom') == 0) {
                        $(itemsSpan[currNo]).slideDown((timeOut / 6), function () {
                            $(items[currNo]).fadeOut((timeOut / 6), function () {
                                faderStat = true;
                                current = items[(currNo + 1)];
                                if (!mOver) {
                                    fadeElement(false);
                                }
                            });
                        });
                    } else {
                        $(itemsSpan[currNo]).slideUp((timeOut / 6), function () {
                            $(items[currNo]).fadeOut((timeOut / 6), function () {
                                faderStat = true;
                                current = items[(currNo + 1)];
                                if (!mOver) {
                                    fadeElement(false);
                                }
                            });
                        });
                    }
                }
            }
        }

	var currNo = 0;
	$(items[currNo]).fadeIn(1,function(){
		var firstheight = $(items[currNo]).find("img").height();
		$(".s3slider").height(firstheight);
		$(".main_visual").height(firstheight);
	});

        makeSlider();

    };

})(jQuery);  
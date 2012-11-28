/*********************
//* jQuery Multi Level CSS Menu #2- By Dynamic Drive: http://www.dynamicdrive.com/
//* Created: Nov 2nd, 08'
//* Menu avaiable at DD CSS Library: http://www.dynamicdrive.com/style/
*********************/
//Specify full URL to down and right arrow images (23 is padding-right to add to top level LIs with drop downs):
var arrowimages = { down: ['downarrowclass', '/images/0.gif'/*/images/jqueryslidemenu/down.gif*/, 23], right: ['rightarrowclass', '/images/jqueryslidemenu/right.gif'] }
var jqueryslidemenu = {
    animateduration: { over: 200, out: 100 }, //duration of slide in/ out animation, in milliseconds
    buildmenu: function (mselector, arrowsvar) {
        jQuery(document).ready(function ($) {
            $("body").click(function (event) {
                var $targetul = $(".jqueryslidemenu li").children("ul:eq(0)");
                if ($targetul.parent().hasClass("over")) {                    
                    $targetul.fadeOut(jqueryslidemenu.animateduration.out);
                    $targetul.parent().removeClass("over");                    
                }
            });
            var $mainmenu = $(mselector)
            var $headers = $mainmenu.find("ul").parent()
            $headers.each(function (i) {
                var $curobj = $(this)
                var $subul = $(this).find('ul:eq(0)')
                this._dimensions = { w: this.offsetWidth, h: this.offsetHeight, subulw: $subul.outerWidth(), subulh: $subul.outerHeight() }
                this.istopheader = $curobj.parents("ul").length == 1 ? true : false
                //$subul.css({ top: this.istopheader ? this._dimensions.h + "px" : 0 })
                if ($curobj.children("a").attr("href") == "#") {
                    $curobj.click(
                        function (e) {
                            e.stopPropagation()
                            var $targetul = $(this).children("ul:eq(0)")
                            //alert($targetul.parent().hasClass("over"));
                            switch ($targetul.parent().hasClass("over")) {
                                case false:
                                    this._offsets = { left: $(this).offset().left, top: $(this).offset().top }
                                    var menuleft = this.istopheader ? 0 : this._dimensions.w
                                    menuleft = (this._offsets.left + menuleft + this._dimensions.subulw > $(window).width()) ? (this.istopheader ? -this._dimensions.subulw + this._dimensions.w : -this._dimensions.w) : menuleft
                                    if (this._dimensions.subulw > this._dimensions.w) {
                                        $targetul.css({ left: menuleft + "px", "min-width": "99%", width: "auto" }).fadeIn(jqueryslidemenu.animateduration.over)
                                    } else {
                                        $targetul.css({ left: menuleft + "px", width: Number(this._dimensions.w - 2) + 'px' }).fadeIn(jqueryslidemenu.animateduration.over)
                                    }
                                    //$targetul.css({left:menuleft+"px", width:'auto'}).slideDown(jqueryslidemenu.animateduration.over)
                                    //$targetul.css({left:menuleft+"px", width:this._dimensions.subulw+'px'}).slideDown(jqueryslidemenu.animateduration.over)
                                    //window.alert(this._dimensions.subulw);
                                    //window.alert( $targetul.css("height") );
                                    $targetul.parent().addClass("over");
                                    break;
                                case true:
                                    var $targetul = $(this).children("ul:eq(0)")
                                    $targetul.fadeOut(jqueryslidemenu.animateduration.out)
                                    //$targetul.slideUp(jqueryslidemenu.animateduration.out)
                                    $targetul.parent().removeClass("over");
                                    break;
                            }
                        }
                    ) //end click
                } else {
                }
            }) //end $headers.each()
            $mainmenu.find("ul").css({ display: 'none', visibility: 'visible' });
        }) //end document.ready
    }
}

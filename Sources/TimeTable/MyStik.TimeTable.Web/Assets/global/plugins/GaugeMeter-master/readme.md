#**GaugeMeter.js** 

An elegant and dynamic animated graphical gauge meter built with jQuery. GaugeMeter.js is highly customizable and includes full-radial, semi-radial &amp; arch dials.

[![GaugeMeter.js jQuery Plugin by Ash Alom](http://ashalom.com/developer/js/GaugeMeter/ScreenShot@640.png)](http://ashalom.github.io/GaugeMeter)

> **DEMO:** http://ashalom.github.io/GaugeMeter

> **REPO:** https://github.com/AshAlom/GaugeMeter

Copyright AshAlom.com 2011. All rights Reserved.

---

####  GaugeMeter.js Features
 - Just one script, there is no dependencies.
 - Lots of built-in themes.
 - HTML5 renders the Canvas content without images.
 - Highly customizable and extensible.
 - Rich feature sets.
 - Add custom legends & labels.
 - Robust & polished.
 - Support for any page size and page color.
 - Configure via data attributes or group settings.

####  Requirements
 - [jQuery 1.10.2](https://code.jquery.com/jquery-1.10.2.min.js)
 - HTML5 compatible web browser.

---

#### Implementation

Include the following jQuery & JavaScript Code, CSS and HTML code to render a minimal form of the Ash Alom GaugeMeter...

**jQuery & JavaScript Code**
```html
<script src="./jquery.AshAlom.gaugeMeter-2.0.0.min.js"></script>
<script>
	$(document).ready(function(){
		$(".GaugeMeter").gaugeMeter();
	});
</script">
```

**CSS Styles**
```css
.GaugeMeter{
	Position:        Relative;
	Text-Align:      Center;
	Overflow:        Hidden;
	Cursor:          Default;
}

.GaugeMeter SPAN,
    .GaugeMeter B{
    	Margin:          0 23%;
    	Width:           5%;
    	Position:        Absolute;
    	Text-align:      Center;
    	Display:         Inline-Block;
    	Color:           RGBa(0,0,0,.8);
    	Font-Weight:     100;
    	Font-Family:     "Open Sans", Arial;
    	Overflow:        Hidden;
    	White-Space:     NoWrap;
    	Text-Overflow:   Ellipsis;
}
.GaugeMeter[data-style="Semi"] B{
	Margin:          0 10%;
	Width:           80%;
}

.GaugeMeter S,
    .GaugeMeter U{
    	Text-Decoration: None;
    	Font-Size:       .49em;
    	Opacity:         .5;
}

.GaugeMeter B{
	Color:           Black;
	Font-Weight:     300;
	Opacity:         .8;
}
```

**HTML Code**

Basic Implementation. The code below is all you will need to render a basic gauge meter.
```html
<div class="GaugeMeter" id="GaugeMeter_1" data-percent="10"></div>
```

Below is a list of all the optional parameters, see the Parameter Definitions for more details on how to utilize these data attributes.
```html
<div class="GaugeMeter" id="GaugeMeter_1" data-percent="10"
	data-text="Spendings"
	data-total="1024"
	data-used="256"
	data-prepend="$"
	data-append=".00"
	data-size="200"
	data-width="2"
	data-style="Semi"
	data-color="Blue"
	data-back="Silver"
	data-theme="Red-Gold-Green"
	data-animate_gauge_colors="1"
	data-animate_text_colors="1"
	data-label="VISA Card"
	data-label_color="#F00"
	data-stripe="2"
></div>
```

----

#### Parameter Definitions

The form of the gauge meter can be manipulated by means of the following parameters. These parameters can be passed in to the library via HTML5 tag data attributes, as illustrated in the HTML example code, above. The following table elaborates upon each of the parameter properties.



Attribute | Optional | Defaults | Values | Description
---|:---:|:---:|---|---
***```data-percent```***|No|0|Any positive integer, between 0 to 100.|The value to set the gauge meter to.
***```data-used```***|Yes|0|Any positive integer.|Display a percentage of a value that overrides any ***```data-percent defined count. To show "25%" of 512 GB of RAM being used, you would specify "128" here and "512" for ***```data-total. ***```data-total```***|Yes|100|Any positive integer.|Display a percentage of a value that overrides any ***```data-percent defined count. To show "25%" of 512 GB of RAM being used, you would specify "512" here and "128" for ***```data-used.
***```data-text```***|Yes|null|Any short string.|Replaces the ***```data-percent count in the center of the gauge.
***```data-prepend```***|Yes|null|Any string (2 bytes max).|Adds this text before the percent count in the center of the gauge.
***```data-append```***|Yes|null|Any string (2 bytes max).|Adds this text after the percent count in the center of the gauge. Typical use would be a "%" symbol.
***```data-size```***|Yes|100|Any positive integer.|Width & height of the gauge meter in pixels.
***```data-width```***|Yes|3|Any positive integer.|Thickness of the gauge meter progress bar in pixels.
***```data-style```***|Yes|Full|Full, Semi or Arch|Displays either a full circle, semi-circle or an arched-circle.
***```data-color```***|Yes|#2C94E0|Hex values (#FFFFFF), Red-Green-Blue-Alpha color space (RGBa(255,255,255,1.0)) or HTML color-name (Red)|The foreground-color of the gauge meter's progress bar. This value is overridden if ***```data-theme is specified. ***```data-back```***|Yes|RGBa(0,0,0,.06)|Hex values (#FFFFFF), Red-Green-Blue-Alpha color space (RGBa(255,255,255,1.0)) or HTML color-name (Green)|The background-color of the gauge meter's progress bar.
***```data-theme```***|Yes|Red-Gold-Green|<ul><li>Red-Gold-Green</li><li>Green-Gold-Red</li><li>Green-Red</li><li>Red-Green</li><li>DarkBlue-LightBlue</li><li>LightBlue-DarkBlue</li><li>DarkRed-LightRed</li><li>LightRed-DarkRed</li><li>DarkGreen-LightGreen</li><li>LightGreen-DarkGreen</li><li>DarkGold-LightGold</li><li>LightGold-DarkGold</li><li>White</li><li>Black</li></ul>|Color & gradient themes to fill the foreground-color of the gauge meter's progress bar with.
***```data-animate_gauge_colors```***|Yes|0|Boolean, 0 or 1.|When enabled, the foreground-color of the gauge meter's progress bar will cycle according to the color value, as directed by the ***```data-theme. If enabled, this overrides any values specified by the ***```data-color. ***```data-animate_text_colors```***|Yes|0|Boolean, 0 or 1.|When enabled, the percentage text color of the gauge meter will cycle according to the color value, as directed by the ***```data-theme. If enabled, this overrides any values specified by the ***```data-label_color
***```data-label```***|Yes|null|Any short string.|Supplemental text label that can appear below the central percentage or text of the gauge meter.
***```data-label_color```***|Yes|Black|Hex values (#FFFFFF), Red-Green-Blue-Alpha color space (RGBa(255,255,255,1.0)) or HTML color-name (Blue)|The foreground text color of the supplemental text label.
***```data-stripe```***|Yes|0|Any positive integers.|Show the gauge meter's progress bar in solid form or stripe form. If the value is greater than 0, the gauge meter's progress bar changes from a solid to a stripe, where the value is the thickness of the stripes.


----

#### Changelog
**2.0.0** - 21st October, 2015 @ 4:29 PM PST

 - Initial public release

**1.0.0** - 11th June, 2012

 - Initial developer release.

**0.0.1** - 9th December, 2011

 - Initial development.

----

#### License
MIT

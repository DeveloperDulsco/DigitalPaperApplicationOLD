let sheet = (function () {
    let style = document.createElement(& quot; style & quot;);
    style.appendChild(document.createTextNode(& quot;& quot;));
    document.head.appendChild(style);
    return style.sheet;
})();
function editToolBar() {
    //when the page is resized, the viewer hides and move some buttons around.
    //this function forcibly show all buttons so none of them disappear or re-appear on page resize
    removeGrowRules();

    /* Reorganizing the UI 
     the &#39;addElemFromSecondaryToPrimary&#39; function moves items from the secondary nav into the primary nav
     there are 3 primary nav regions (toolbarViewerLeft, toolbarViewerMiddle, toolbarViewerRight) 
    */

    //adding elements to left part of toolbar
    addElemFromSecondaryToPrimary(&#39; pageRotateCcw &#39;, &#39; toolbarViewerLeft &#39;)
    addElemFromSecondaryToPrimary(&#39; pageRotateCw &#39;, &#39; toolbarViewerLeft &#39;)
    addElemFromSecondaryToPrimary(&#39; zoomIn &#39;, &#39; toolbarViewerLeft &#39;)
    addElemFromSecondaryToPrimary(&#39; zoomOut &#39;, &#39; toolbarViewerLeft &#39;)

    //adding elements to middle part of toolbar
    addElemFromSecondaryToPrimary(&#39; previous &#39;, &#39; toolbarViewerMiddle &#39;)
    addElemFromSecondaryToPrimary(&#39; pageNumber &#39;, &#39; toolbarViewerMiddle &#39;)
    addElemFromSecondaryToPrimary(&#39; numPages &#39;, &#39; toolbarViewerMiddle &#39;)
    addElemFromSecondaryToPrimary(&#39; next &#39;, &#39; toolbarViewerMiddle &#39;)

    //adding elements to right part of toolbar
    addElemFromSecondaryToPrimary(&#39; secondaryOpenFile &#39;, &#39; toolbarViewerRight &#39;)

    /* Changing icons */
    changeIcon(&#39; previous &#39;, &#39; icons / baseline - navigate_before - 24px.svg &#39;)
    changeIcon(&#39; next &#39;, &#39; icons / baseline - navigate_next - 24px.svg &#39;)
    changeIcon(&#39; pageRotateCcw &#39;, &#39; icons / baseline - rotate_left - 24px.svg &#39;)
    changeIcon(&#39; pageRotateCw &#39;, &#39; icons / baseline - rotate_right - 24px.svg &#39;)
    changeIcon(&#39; viewFind &#39;, &#39; icons / baseline - search - 24px.svg &#39;);
    changeIcon(&#39; zoomOut &#39;, &#39; icons / baseline - zoom_out - 24px.svg &#39;)
    changeIcon(&#39; zoomIn &#39;, &#39; icons / baseline - zoom_in - 24px.svg &#39;)
    changeIcon(&#39; sidebarToggle &#39;, &#39; icons / baseline - toc - 24px.svg &#39;)
    changeIcon(&#39; secondaryOpenFile &#39;, &#39;./icons/baseline - open_in_browser - 24px.svg &#39;)

    /* Hiding elements */
    removeElement(&#39; secondaryToolbarToggle &#39;)
    removeElement(&#39; scaleSelectContainer &#39;)
    removeElement(&#39; presentationMode &#39;)
    removeElement(&#39; openFile &#39;)
    removeElement(&#39; print &#39;)
    removeElement(&#39; download &#39;)
    removeElement(&#39; viewBookmark &#39;)
}
function changeIcon(elemID, iconUrl) {
    let element = document.getElementById(elemID);
    let classNames = element.className;
    classNames = elemID.includes(&#39; Toggle &#39;)? &#39; toolbarButton# &#39; +elemID :
    classNames.split(&#39; &#39;).join(&#39;.&#39;);
    classNames = elemID.includes(&#39; view &#39;)? &#39;# &#39; +elemID +&#39;.toolbarButton &#39; : &#39;.&#39; +classNames
    classNames += & quot;:: before & quot;;
    addCSSRule(sheet, classNames, `content: url(${iconUrl}) !important`, 0)
}
function addElemFromSecondaryToPrimary(elemID, parentID) {
    let element = document.getElementById(elemID);
    let parent = document.getElementById(parentID);
    element.style.minWidth = & quot; 0px & quot;;
    element.innerHTML =&#39;&#39;
    parent.append(element);
}
function removeElement(elemID) {
    let element = document.getElementById(elemID);
    element.parentNode.removeChild(element);
}
function removeGrowRules() {
    addCSSRule(sheet, &#39;.hiddenSmallView *&#39;, &#39; display: block!important &#39;);
    addCSSRule(sheet, &#39;.hiddenMediumView &#39;, &#39; display: block!important &#39;);
    addCSSRule(sheet, &#39;.hiddenLargeView &#39;, &#39; display: block!important &#39;);
    addCSSRule(sheet, &#39;.visibleSmallView &#39;, &#39; display: block!important &#39;);
    addCSSRule(sheet, &#39;.visibleMediumView &#39;, &#39; display: block!important &#39;);
    addCSSRule(sheet, &#39;.visibleLargeView &#39;, &#39; display: block!important &#39;);
}
function addCSSRule(sheet, selector, rules, index) {
    if (& quot; insertRule & quot; in sheet) {
        sheet.insertRule(selector + & quot; {& quot; + rules + & quot; }& quot;, index);
    }
  else if (& quot; addRule & quot; in sheet) {
        sheet.addRule(selector, rules, index);
    }
}
window.onload = editToolBar
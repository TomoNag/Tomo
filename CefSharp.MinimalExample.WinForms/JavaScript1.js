function getElementsByAttribute(attribute, context) {
    var nodeList = (context || document).getElementsByTagName('*');
    var nodeArray = [];
    var iterator = 0;
    var node = null;
    var count = 0;
    while (node = nodeList[iterator++]) {
        if (node.getAttribute(attribute)) {
            var s = node.getAttribute(attribute);
            if (s.indexOf("javascript") > -1) {
                if (node.textContent.indexOf("動画") > -1) {

                    node.click();
                    count++;
                    nodeArray.push(node);
                }
            }
        }

    }
}

getElementsByAttribute('href');
import cytoscape from "cytoscape";

export async function nodeMouseOver(e: cytoscape.EventObject) {
    const node = e.target;
    let popper = node.popper({
        content: () => {
            let div = document.createElement('div');

            div.innerHTML = node.data('parent_name') ?? 'no content';
            div.style.backgroundColor = 'white'
            div.style.zIndex = '1000';
            div.style.padding = '10px'
            div.style.border = 'solid black 2px'

            document.body.appendChild(div);
            setTimeout(() => {
                document.body.removeChild(div);
            }, 500);

            return div;
        },
        popper: {} // my popper options here
    });

    let update = () => {
        //@ts-ignore
        popper.update();
    };

    node.on('position', update);
    node.on('mouseout', () => {
    });

    window.cy?.on('pan zoom resize', update);
}

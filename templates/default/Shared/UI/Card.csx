using CSX.Runtime;

namespace Shared.UI;

component Card {
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    return 
    <div class={Css.Card}>
        <h3>{Title}</h3>
        {ChildContent}
    </div>;
}

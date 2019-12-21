import { Component, Input } from '@angular/core';

@Component({
    selector: 'tab',
    styles: [`
    .pane{
      padding: 1em;
    }
  `],
    template: `
    <div [hidden]="!active" class="pane" style="padding:0px;">
      <ng-content></ng-content>
    </div>
  `
})
export class Tab {
    @Input('tabTitle') title: string;
    @Input('tabActive') active = false;
}

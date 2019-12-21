import { Component, ContentChildren, QueryList, AfterContentInit, Output, EventEmitter } from '@angular/core';
import { Tab } from './tab';

@Component({
    selector: 'tabs',
    template: `
    <ul class="nav nav-tabs">
      <li *ngFor="let tab of tabs" (click)="selectTab(tab)" [class.active]="tab.active" style="padding-right:10px;">
        <a href="#" onclick="return false;">{{tab.title}}</a>
      </li>
    </ul>
    <ng-content></ng-content>
  `,
    //styleUrls: ['./bootstrap.min.css']
    
})
export class Tabs implements AfterContentInit {
  @Output('tabChanged') changed: EventEmitter<string> = new EventEmitter<string>();
    @ContentChildren(Tab) tabs: QueryList<Tab>;

    // contentChildren are set
    ngAfterContentInit() {
        // get all active tabs
        //let activeTabs = this.tabs.filter((tab) => tab.active);

        //// if there is no active tab set, activate the first
        //if (activeTabs.length === 0) {
        //    this.selectTab(this.tabs.first);
        //}
    }

    selectTab(tab: Tab) {
        // deactivate all tabs
        this.tabs.toArray().forEach(tab => tab.active = false);

        // activate the tab the user has clicked on.
        tab.active = true;
        this.changed.emit(tab.title);
    }

}


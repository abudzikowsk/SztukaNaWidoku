import {Component, Input} from '@angular/core';
import {NgOptimizedImage} from "@angular/common";
import {ExhibitionModel} from "../models/exhibition.model";
import {Title} from "@angular/platform-browser";

@Component({
  selector: 'app-event-box',
  standalone: true,
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './event-box.component.html',
  styleUrl: './event-box.component.scss'
})
export class EventBoxComponent {
  @Input() event!: ExhibitionModel;
}

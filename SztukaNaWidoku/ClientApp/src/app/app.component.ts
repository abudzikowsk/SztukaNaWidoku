import {Component, HostListener, inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {NavBarComponent} from "./nav-bar/nav-bar.component";
import {EventBoxComponent} from "./event-box/event-box.component";
import {ExhibitionModel} from "./models/exhibition.model";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavBarComponent, EventBoxComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private readonly httpClient = inject(HttpClient);
  private readonly MOBILE_WIDTH = 900;
  title = 'Sztuka na widoku';

  response: ExhibitionModel[] = [];
  isMobile = false;

  ngOnInit(): void {
    this.httpClient.get<ExhibitionModel[]>('/api/exhibitions').subscribe(result => {
      this.response = result;
    }, error => console.error(error));

    this.isMobile = window.innerWidth <= this.MOBILE_WIDTH;
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.isMobile = window.innerWidth <= this.MOBILE_WIDTH;
  }
}

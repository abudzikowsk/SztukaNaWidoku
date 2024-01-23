import {Component, inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {NavBarComponent} from "./nav-bar/nav-bar.component";
import {EventBoxComponent} from "./event-box/event-box.component";
import {ExhibitionModel} from "./models/exhibition.model";
import {HttpClient, HttpClientModule} from "@angular/common/http";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavBarComponent, EventBoxComponent, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  title = 'Sztuka na widoku';
  httpClient = inject(HttpClient);

  response: ExhibitionModel[] = [];
  ngOnInit(): void {
    this.httpClient.get<ExhibitionModel[]>('/api/exhibitions').subscribe(result => {
      this.response = result;
    }, error => console.error(error));
  }
}

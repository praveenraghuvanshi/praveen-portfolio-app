import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  contactForm: FormGroup;
  name: FormControl;
  email: FormControl;
  subject: FormControl;
  message: FormControl;

  constructor(private http: HttpClient) {
    this.name = new FormControl();
    this.email = new FormControl();
    this.subject = new FormControl();
    this.message = new FormControl();

    this.contactForm = new FormGroup({
      'name' : this.name,
      'email' : this.email,
      'subject' : this.subject,
      'message' : this.message
    });

   }

  ngOnInit(): void {    
  }

  onSubmit(form: FormGroup) {
    console.log('Name', form.value.name);
    console.log('Email', form.value.email);
    console.log('Message', form.value.message);

    let body = {
      name : form.value.name,
      email : form.value.email,
      subject : form.value.subject,
      message : form.value.message
    }

    // Simple POST request with a JSON body and response type <any>
    this.http.post<any>('api/contact', body).subscribe(data => {
      console.log(data);
      console.log("Contact");
  })
  }

}

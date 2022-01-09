import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

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

  constructor() {
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
  }

}

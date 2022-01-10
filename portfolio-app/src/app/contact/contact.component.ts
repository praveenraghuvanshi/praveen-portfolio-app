import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

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

  response: string = '';

  constructor(private http: HttpClient, private toastr: ToastrService) {
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

  success(message:string): void {
    this.toastr.success(message);
  }

  error(message:string): void {
    this.toastr.error(message);
  }

  onSubmit(form: FormGroup) {
    console.log('Name', form.value.name);
    console.log('Email', form.value.email);
    console.log('Message', form.value.message);

    let body = {
      application : "portfolio",
      name : form.value.name,
      email : form.value.email,
      subject : form.value.subject,
      message : form.value.message
    }

    // Simple POST request with a JSON body and response type <any>
    this.http.post('api/contact', body, {responseType: 'json'}).subscribe({
      next: data => {
          console.log("success");
          console.log(JSON.stringify(data));
          this.success("Submitted successfully");
      },
      error: error => {
          console.error('There was an error!', error);
          this.error("Error occured while submitting");
      }
  })
}

}

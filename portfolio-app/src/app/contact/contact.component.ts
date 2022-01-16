import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  application:string = "WebApp";
  contactForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private http: HttpClient, private toastr: ToastrService) {
    this.contactForm = this.formBuilder.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      subject: ['', Validators.required],
      message: ['']
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
    // stop here if form is invalid
    if (this.contactForm.invalid) {
      return;
    }

    let body = {
      application : this.application,
      name : form.value.name,
      email : form.value.email,
      subject : form.value.subject,
      message : form.value.message
    }

    // Simple POST request with a JSON body and response type <any>
    this.http.post('api/contact', body, {responseType: 'json'}).subscribe({
      next: data => {
          console.log("success");
          this.success("Submitted successfully");
      },
      error: error => {
          console.error('There was an error!', error);
          this.error("Error occured while submitting");
      }
    })
  }
}
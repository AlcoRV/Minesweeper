import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpService } from '../../core/services/http.service';
import { NewGameRequest } from '../../core/models/Requests';

@Component({
  selector: 'app-creation-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './creation.form.component.html',
  styleUrl: './creation.form.component.css'
})
export class CreationFormComponent {

  form: FormGroup<{ 
    width: FormControl<number | null>;
    height: FormControl<number | null>;
    minesCount: FormControl<number | null>;
  }>;

  constructor(private fb: FormBuilder, private formDataService: HttpService) {

    this.form = this.fb.group({
      width: new FormControl(0, [Validators.required, Validators.min(1)]),
      height: new FormControl(0, [Validators.required, Validators.min(1)]),
      minesCount: new FormControl(0, [Validators.required, Validators.min(1)]),
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const data: NewGameRequest = {
        height: this.form.value.height ?? 0,
        width: this.form.value.width ?? 0,
        mines_count: this.form.value.minesCount ?? 0,
      };
      console.log('Form data submitted: ', data);
      this.formDataService.generateBoard(data);
    } else {
      console.log('Form is invalid!');
    }
  }
}

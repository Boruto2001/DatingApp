import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';
import { UserParams } from 'src/app/_models/userParams';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
 members:Member[];
 pagination:Pagination;
 userParams: UserParams;
 pageNumber=1;
 pageSize=5;
  constructor(private memberService:MembersService) { }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.pageNumber,this.pageSize).subscribe(
      response=>{
        this.members=response.result;
        this.pagination=response.pagination;
      }
    )
  }
 

  pageChanged(event: any) {
    // this.userParams.pageNumber = event.page;
    // this.memberService.setUserParams(this.userParams);
    this.pageNumber=event.page;
    this.loadMembers();
  }
 
}
